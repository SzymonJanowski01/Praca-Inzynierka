import sys
import os
from uuid import uuid4
from typing import List, Dict, Union
from datetime import datetime

from sqlalchemy import Column, String, select, ForeignKey, or_, DateTime, and_, desc
from sqlalchemy.orm import relationship
from sqlalchemy.exc import NoResultFound
from sqlalchemy.ext.asyncio import AsyncSession

from app.services.database import Base
from app.services.password_operations import password_hashing, password_check
from app.supporting_functions import match_champions_with_ids, match_champions_with_names

from RecommendationSystem.main import get_recommendations


class User(Base):
    __tablename__ = "users"
    user_id = Column(String, primary_key=True)
    username = Column(String, unique=True, nullable=False)
    email = Column(String, unique=True, nullable=False)
    password = Column(String, nullable=False)
    salt = Column(String, nullable=False)

    scenarios = relationship("Scenario", cascade="all, delete-orphan")

    @classmethod
    async def create_user(cls, db: AsyncSession, username: str, email: str, password: str):
        """
        Creates a new user and adds it to the database.

        :param db: Async session from sessionmanager
        :param username: User username
        :param email: User email
        :param password: User password
        :return: User object (user_id: generated hex, username: str, email: str, password: str, salt: str) if username and email are not already taken, otherwise raises ValueError
        """
        user_id = uuid4().hex

        check_for_existing_username = (await db.execute(select(cls).where(cls.username == username))).scalars().first()
        if check_for_existing_username:
            raise ValueError("Username already taken")

        check_for_existing_email = (await db.execute(select(cls).where(cls.email == email))).scalars().first()
        if check_for_existing_email:
            raise ValueError("Email already taken")

        password_data = password_hashing(password)

        hashed_password = password_data["hashed_password"].hex()
        salt = password_data["salt"].hex()

        user = cls(user_id=user_id, username=username, email=email, password=hashed_password, salt=salt)
        db.add(user)

        await db.commit()
        await db.refresh(user)

        return user

    @classmethod
    async def get_user(cls, db: AsyncSession, user_id: str):
        """
        Function for getting a single user from the database.

        :param db: Async session from sessionmanager
        :param user_id: User id
        :return: User object (user_id: str, username: str, email: str, password: str, salt: str) if user exists, otherwise None
        """
        try:
            user = await db.get(cls, user_id)
        except NoResultFound:
            return None
        return user

    @classmethod
    async def get_all_users(cls, db: AsyncSession):
        """
        Function for getting all users from the database.

        :param db: Async session from sessionmanager
        :return: Set of User objects (user_id: str, username: str, email: str, password: str, salt: str)
        """
        return (await db.execute(select(cls))).scalars().all()

    @classmethod
    async def verify_credentials(cls, db: AsyncSession, username_or_email: str, provided_password: str):
        """
        Function for verifying user credentials.

        :param db: Async session from sessionmanager
        :param username_or_email: User username or email
        :param provided_password: Password to run a check on
        :return: ID of the user if credentials are valid, False if credentials are invalid, None if user does not exist
        """
        user = (await db.execute(select(cls).where(
            or_(cls.username == username_or_email, cls.email == username_or_email)))).scalars().first()

        if not user:
            return None

        password_as_bytes = bytes.fromhex(user.password)
        salt_as_bytes = bytes.fromhex(user.salt)

        if password_check(provided_password, password_as_bytes, salt_as_bytes):
            return user.user_id
        else:
            return False

    @classmethod
    async def update_user(cls, db: AsyncSession, user_id: str, new_username: str = None, new_email: str = None,
                          new_password: str = None):
        """
        Function for updating user data.

        :param db: Async session from sessionmanager
        :param user_id: ID of the user to update
        :param new_username: New username to set (optional)
        :param new_email: New email to set (optional)
        :param new_password: New password to set (optional)
        :return: Updated User object (user_id: str, username: str, email: str, password: str, salt: str) if user exists, otherwise None, might raise ValueError if username or email are already taken
        """
        user = await db.get(cls, user_id)

        if not user:
            return None

        if new_username:
            existing_user = (await db.execute(select(cls).where(cls.username == new_username))).scalars().first()
            if existing_user:
                raise ValueError(f"Username '{new_username}' is already taken")

            user.username = new_username

        if new_email:
            existing_email = (await db.execute(select(cls).where(cls.email == new_email))).scalars().first()
            if existing_email:
                raise ValueError(f"Email '{new_email}' is already taken")

            user.email = new_email

        if new_password:
            password_data = password_hashing(new_password)

            user.password = password_data["hashed_password"].hex()
            user.salt = password_data["salt"].hex()

        await db.commit()
        await db.refresh(user)
        return user

    @classmethod
    async def delete_user(cls, db: AsyncSession, user_id: str):
        """
        Function for deleting a user from the database.

        :param db: Async session from sessionmanager
        :param user_id: ID of the user to delete
        :return: True if user was deleted, otherwise None
        """
        user = await cls.get_user(db, user_id)

        if not user:
            return None

        await db.delete(user)
        await db.commit()
        return True


class Scenario(Base):
    __tablename__ = "scenarios"
    scenario_id = Column(String, primary_key=True)
    user_id = Column(String, ForeignKey('users.user_id', ondelete="CASCADE"), nullable=False)
    name = Column(String, nullable=False)
    created_at = Column(DateTime, default=datetime.utcnow)
    last_modified_at = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)

    phases = relationship("Phase", cascade="all, delete-orphan")

    @classmethod
    async def create_scenario(cls, db: AsyncSession, user_id: str, name: str):
        """
        Creates a new scenario and adds it to the database.

        :param db: Async session from sessionmanager
        :param user_id: ID of the user who created the scenario
        :param name: Name of the scenario
        :return: Scenario object (scenario_id: generated hex, user_id: str, name: str, created_at: datetime, last_modified_at: datetime)
        """
        scenario_id = uuid4().hex

        scenario = cls(scenario_id=scenario_id, user_id=user_id, name=name)
        db.add(scenario)

        await db.commit()
        await db.refresh(scenario)
        return scenario

    @classmethod
    async def get_scenarios_names(cls, db: AsyncSession, user_id: str):
        """
        Function for getting all scenarios names associated with the user.

        :param db: Async session from sessionmanager
        :param user_id: ID of the user to get scenarios names for
        :return: Set of scenarios names (str)
        """
        scenarios_names = (await db.execute(select(cls.name).where(cls.user_id == user_id))).scalars().all()

        return scenarios_names

    @classmethod
    async def get_all_user_scenarios(cls, db: AsyncSession, user_id: str, paging, filter_param: str = None):
        """
        Function for getting all scenarios associated with the user, with built-in paging and filtering

        .
        :param db: Async session from sessionmanager
        :param user_id: ID of the user to get scenarios for
        :param paging: Paging object
        :param filter_param: Filter parameter (optional)
        :return: Set of Scenario objects (scenario_id: str, user_id: str, name: str, created_at: datetime, last_modified_at: datetime)
        """
        query = select(cls).where(cls.user_id == user_id)

        if filter_param:
            query = query.where(and_(cls.name.like(f"%{filter_param}%")))

        query = query.order_by(desc(cls.last_modified_at))
        query = query.offset(paging.skip).limit(paging.limit)

        scenarios = (await db.execute(query)).scalars().all()

        return scenarios

    @classmethod
    async def update_scenario(cls, db: AsyncSession, scenario_id: str, new_name: str):
        """
        Function for updating scenario data.

        :param db: Async session from sessionmanager
        :param scenario_id: ID of the scenario to update
        :param new_name: New name to set
        :return: Scenario object (scenario_id: str, user_id: str, name: str, created_at: datetime, last_modified_at: datetime) if scenario exists, otherwise None
        """
        try:
            scenario = await db.get(cls, scenario_id)
        except NoResultFound:
            return None

        scenario.name = new_name

        await db.commit()
        await db.refresh(scenario)
        return scenario

    @classmethod
    async def delete_scenario(cls, db: AsyncSession, scenario_id: str):
        """
        Function for deleting a scenario from the database.

        :param db: Async session from sessionmanager
        :param scenario_id: ID of the scenario to delete
        :return: True if scenario was deleted, otherwise None
        """
        scenario = await db.get(cls, scenario_id)

        if not scenario:
            return None

        await db.delete(scenario)
        await db.commit()

        return True


class Phase(Base):
    __tablename__ = "phases"
    phase_id = Column(String, primary_key=True)
    scenario_id = Column(String, ForeignKey("scenarios.scenario_id", ondelete="CASCADE"), nullable=False)
    name = Column(String, nullable=False)
    main_character = Column(String, nullable=False)
    first_alternative_character = Column(String)
    second_alternative_character = Column(String)

    @classmethod
    async def create_empty_phases(cls, db: AsyncSession, scenario_id: str) -> list[str]:
        """
        Creates 10 empty phases and adds them to the database.

        :param db: Async session from sessionmanager
        :param scenario_id: ID of the scenario to create phases for
        :return: List of phase IDs
        """
        phase_ids = []

        # Create 5 phases named B1 to B5 for blue team
        for i in range(1, 6):
            phase_id = uuid4().hex
            name = f"B{i}"
            phase = cls(phase_id=phase_id, scenario_id=scenario_id, name=name, main_character="None",
                        first_alternative_character="None", second_alternative_character="None")
            phase_ids.append(phase_id)
            db.add(phase)

        # Create 5 phases named R1 to R5 for red team
        for i in range(1, 6):
            phase_id = uuid4().hex
            name = f"R{i}"
            phase = cls(phase_id=phase_id, scenario_id=scenario_id, name=name, main_character="None",
                        first_alternative_character="None", second_alternative_character="None")
            phase_ids.append(phase_id)
            db.add(phase)

        await db.commit()

        return phase_ids

    @classmethod
    async def get_all_scenario_phases(cls, db: AsyncSession, scenario_id: str):
        """
        Function for getting all phases associated with the scenario.

        :param db: Async session from sessionmanager
        :param scenario_id: ID of the scenario to get phases for
        :return: Set of Phase objects (phase_id: str, scenario_id: str, name: str, main_character: str, first_alternative_character: str, second_alternative_character: str) sorted by name (B1, B2, ..., B5, R1, R2, ..., R5)
        """
        phases = (await db.execute(select(cls).where(cls.scenario_id == scenario_id))).scalars().all()

        phases = sorted(phases, key=lambda phase: phase.name)

        return phases

    @classmethod
    async def update_scenario_phases(cls, db: AsyncSession, scenario_id: str,
                                     phases_changes: List[Dict[str, Union[int, Dict[str, str]]]]):
        """
        Function for updating scenario phases data.

        :param db: Async session from sessionmanager
        :param scenario_id: ID of the scenario to update phases for
        :param phases_changes: List of dictionaries with phase index and attributes to update
        :return: Set of Phase objects (phase_id: str, scenario_id: str, name: str, main_character: str, first_alternative_character: str, second_alternative_character: str) sorted by name (B1, B2, ..., B5, R1, R2, ..., R5) if scenario exists, otherwise None, might raise ValueError if phase index or attribute are invalid
        """
        # Get all phases associated with the scenario
        phases = (await db.execute(select(cls).where(cls.scenario_id == scenario_id))).scalars().all()

        if not phases:
            return None

        # Sort phases by name (B1, B2, ..., B5, R1, R2, ..., R5)
        phases = sorted(phases, key=lambda phase: phase.name)

        scenario = await db.get(Scenario, scenario_id)

        for change in phases_changes:
            """
            change = {
                'phase_index': int(0-9),
                'attributes': {
                    'main_character': str (optional),
                    'first_alternative_character': str (optional),
                    'second_alternative_character': str (optional)
                }
            }
            """
            phase_index = change['phase_index']
            attributes = change['attributes']

            if phase_index < 0 or phase_index >= len(phases):
                raise ValueError(f"Invalid phase index: {phase_index}")

            # Get phase to update from list of phases belonging to the scenario
            phase = phases[phase_index]

            # Ensure that phase has provided attributes, and update them with new values. If invalid attribute is provided, raise ValueError
            for attr, value in attributes.items():
                if hasattr(phase, attr):
                    setattr(phase, attr, value)
                else:
                    raise ValueError(f"Invalid attribute: {attr}")

        # Update scenario's last_modified_at attribute with current datetime to indicate that scenario was updated
        scenario.last_modified_at = datetime.utcnow()

        await db.commit()

        return phases

    @classmethod
    async def get_recommendations(cls, db: AsyncSession, user_id: str, champions_list: list[str],
                                  target_position: str) -> Union[dict | None]:

        users_ids = (await db.execute(select(User.user_id))).scalars().all()
        if user_id not in users_ids:
            return None

        champions_ids: list[int] = match_champions_with_ids(champions_list)

        recommendation: dict[str, list[int]] = get_recommendations(champions_ids, target_position)

        final_recommendation = match_champions_with_names(recommendation)

        return final_recommendation
