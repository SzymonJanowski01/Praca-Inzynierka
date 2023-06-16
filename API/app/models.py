from uuid import uuid4
from typing import List, Dict, Union

from sqlalchemy import Column, String, select, ForeignKey, or_
from sqlalchemy.orm import relationship
from sqlalchemy.exc import NoResultFound
from sqlalchemy.ext.asyncio import AsyncSession

from app.services.database import Base
from app.services.password_operations import password_hashing, password_check


class User(Base):
    __tablename__ = "users"
    id = Column(String, primary_key=True)
    username = Column(String, unique=True, nullable=False)
    email = Column(String, unique=True, nullable=False)
    password = Column(String, nullable=False)
    salt = Column(String, nullable=False)

    scenarios = relationship("Scenario", cascade="all, delete-orphan")

    @classmethod
    async def create_user(cls, db: AsyncSession, id=None, **kwargs):
        if not id:
            id = uuid4().hex

        username = kwargs.get("username")

        check_for_existing_username = (await db.execute(select(cls).where(cls.username == username))).scalars().first()
        if check_for_existing_username:
            raise ValueError("Username already taken")

        email = kwargs.get("email")

        check_for_existing_email = (await db.execute(select(cls).where(cls.email == email))).scalars().first()
        if check_for_existing_email:
            raise ValueError("Account with provided email already exists")

        password_data = password_hashing(kwargs.pop("password", None))

        kwargs["password"] = password_data[0]
        kwargs["salt"] = password_data[1]

        transaction = cls(id=id, **kwargs)
        db.add(transaction)

        await db.commit()
        await db.refresh(transaction)

        return transaction

    @classmethod
    async def get_user(cls, db: AsyncSession, id: str):
        try:
            transaction = await db.get(cls, id)
        except NoResultFound:
            return None
        return transaction

    @classmethod
    async def get_all_users(cls, db: AsyncSession):
        return (await db.execute(select(cls))).scalars().all()

    @classmethod
    async def verify_credentials(cls, db: AsyncSession, username_or_email: str, provided_password: str):
        user = (await db.execute(select(cls).where(
            or_(cls.username == username_or_email, cls.email == username_or_email)))).scalars().first()

        if not user:
            return None

        return password_check(provided_password, user.password, user.salt)

    @classmethod
    async def update_user(cls, db: AsyncSession, user_id: str, new_username: str = None, new_email: str = None,
                          new_password: str = None):
        user = await db.get(cls, user_id)

        if not user:
            return None

        if new_username:
            existing_user = (await db.execute(select(cls).where(cls.username == new_username))).scalars().first()
            if existing_user:
                raise ValueError(f"Username '{new_username} is already taken")

            user.username = new_username

        if new_email:
            existing_email = (await db.execute(select(cls).where(cls.email == new_email))).scalars().first()
            if existing_email:
                raise ValueError(f"Email '{new_email}' is already taken")

            user.email = new_email

        if new_password:
            password_data = password_hashing(new_password)

            user.password = password_data[0]
            user.salt = password_data[1]

        await db.commit()
        await db.refresh(user)
        return user

    @classmethod
    async def delete_user(cls, db: AsyncSession, user_id: str):
        user = await cls.get_user(db, user_id)

        if not user:
            return None

        await db.delete(user)
        await db.commit()
        return True


class Scenario(Base):
    __tablename__ = "scenarios"
    id = Column(String, primary_key=True)
    user_id = Column(String, ForeignKey('User.id', ondelete="CASCADE"), nullable=False)
    name = Column(String, nullable=False)

    phases = relationship("Phase", cascade="all, delete-orphan")

    @classmethod
    async def create_scenario(cls, db: AsyncSession, id=None, **kwargs):
        if not id:
            id = uuid4().hex

        transaction = cls(id=id, **kwargs)
        db.add(transaction)
        await db.commit()
        await db.refresh(transaction)
        return transaction

    @classmethod
    async def get_scenario(cls, db: AsyncSession, scenario_id: str):
        try:
            transaction = await db.get(cls, scenario_id)
        except NoResultFound:
            return None
        return transaction

    @classmethod
    async def get_all_user_scenarios(cls, db: AsyncSession, user_id: str):
        try:
            transaction = (await db.execute(select(cls).where(cls.user_id == user_id))).scalars().all()
        except NoResultFound:
            return None
        return transaction

    @classmethod
    async def update_scenario(cls, db: AsyncSession, scenario_id: str, new_name: str):
        try:
            transaction = await db.get(cls, scenario_id)
        except NoResultFound:
            return None

        transaction.name = new_name

        await db.commit()
        await db.refresh(transaction)
        return transaction

    @classmethod
    async def delete_scenario(cls, db: AsyncSession, scenario_id: str):
        scenario = await cls.get_scenario(db, scenario_id)
        if not scenario:
            return None

        await db.delete(scenario)
        await db.commit()

        return True


class Phase(Base):
    __tablename__ = "phases"
    id = Column(String, primary_key=True)
    scenario_id = Column(String, ForeignKey("Scenario.id", ondelete="CASCADE"), nullable=False)
    name = Column(String, nullable=False)
    main_character = Column(String, nullable=False)
    firs_alternative_character = Column(String)
    second_alternative_character = Column(String)

    @classmethod
    async def create_empty_phases(cls, db: AsyncSession, scenario_id: str):
        for i in range(1, 6):
            id = uuid4().hex
            name = f"B{i}"
            Phase = cls(id=id, scenario_id=scenario_id, name=name, main_character="Any")
            db.add(Phase)

        for i in range(1, 6):
            id = uuid4().hex
            name = f"R{i}"
            Phase = cls(id=id, scenario_id=scenario_id, name=name, main_character="Any")
            db.add(Phase)

        await db.commit()

    @classmethod
    async def get_all_scenario_phases(cls, db: AsyncSession, scenario_id: str):
        try:
            transaction = (await db.execute(select(cls).where(cls.scenario_id == scenario_id))).scalars().all()
        except NoResultFound:
            return None
        return transaction

    @classmethod
    async def update_scenario_phases(cls, db: AsyncSession, scenario_id: str,
                                     phases_changes: List[Dict[str, Union[int, Dict[str, str]]]]):
        phases = (await db.execute(select(Phase).where(Phase.scenario_id == scenario_id))).scalars().all()

        for change in phases_changes:
            phase_index = change['phase_index']
            attributes = change['attributes']

            if phase_index < 0 or phase_index >= len(phases):
                raise ValueError(f"Invalid phase index: {phase_index}")

            phase = phases[phase_index]

            for attr, value in attributes.items():
                if hasattr(phase, attr):
                    setattr(phase, attr, value)
                else:
                    raise ValueError(f"Invalid attribute: {attr}")

        await db.commit()

        return phases
