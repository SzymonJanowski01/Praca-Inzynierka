from uuid import uuid4

from sqlalchemy import Column, String, select, ForeignKey
from sqlalchemy.exc import NoResultFound
from sqlalchemy.ext.asyncio import AsyncSession

from app.services.database import Base
from app.services.password_operations import password_hashing


class User(Base):
    __tablename__ = "users"
    id = Column(String, primary_key=True)
    username = Column(String, unique=True, nullable=False)
    email = Column(String, unique=True, nullable=False)
    password = Column(String, nullable=False)
    salt = Column(String, nullable=False)

    @classmethod
    async def create(cls, db: AsyncSession, id=None, **kwargs):
        if not id:
            id = uuid4().hex

        username = kwargs.get("username")

        check_for_existing_username = await db.query(cls).filter_by(username=username).first()
        if check_for_existing_username:
            raise ValueError("Username already taken")

        email = kwargs.get("email")

        check_for_existing_email = await db.query(cls).filter_by(email=email).first()
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
    async def get(cls, db: AsyncSession, id: str):
        try:
            transaction = await db.get(cls, id)
        except NoResultFound:
            return None
        return transaction

    @classmethod
    async def get_all(cls, db: AsyncSession):
        return (await db.execute(select(cls))).scalar().all()


class Scenario(Base):
    __tablename__ = "scenarios"
    id = Column(String, primary_key=True)
    user_id = Column(String, ForeignKey('User.id'), nullable=False)
    name = Column(String, unique=True, nullable=False)

    @classmethod
    async def create(cls, db: AsyncSession, id=None, **kwargs):
        if not id:
            id = uuid4().hex

        transaction = cls(id=id, **kwargs)
        db.add(transaction)
        await db.commit()
        await db.refresh(transaction)
        return transaction

    @classmethod
    async def get(cls, db: AsyncSession, id: str):
        try:
            transaction = await db.get(cls, id)
        except NoResultFound:
            return None
        return transaction

    @classmethod
    async def get_all(cls, db: AsyncSession, user_id: str):
        try:
            transaction = (await db.execute(select(cls).where(cls.user_id == user_id))).scalar().all()
        except NoResultFound:
            return None
        return transaction


class Phase(Base):
    __tablename__ = "phases"
    id = Column(String, primary_key=True)
    scenario_id = Column(String, ForeignKey("Scenario.id"), nullable=False)
    name = Column(String, nullable=False)

    @classmethod
    async def create(cls, db: AsyncSession, id=None, **kwargs):
        if not id:
            id = uuid4().hex

        transaction = cls(id=id, **kwargs)
        db.add(transaction)
        await db.commit()
        await db.refresh(transaction)
        return transaction

    @classmethod
    async def get_all_scenario_phases(cls, db: AsyncSession, scenario_id: str):
        try:
            transaction = (await db.execute(select(cls).where(cls.scenario_id == scenario_id))).scalar().all()
        except NoResultFound:
            return None
        return transaction
