from fastapi import APIRouter, Depends
from pydantic import BaseModel
from sqlalchemy.ext.asyncio import AsyncSession

from app.services.database import get_db
from app.supporting_functions import convert_to_optional

from ..models import User as UserModel

router = APIRouter(prefix="/user", tags=["user"])


class UserSchemaBase(BaseModel):
    username: str
    email: str
    password: bytes


class UserSchemaCreate(UserSchemaBase):
    pass


class UserSchemaUpdate(UserSchemaBase):
    __annotations__ = convert_to_optional(UserSchemaBase)


class UserSchema(UserSchemaBase):
    id: str
    salt: bytes

    class Config:
        orm_mode = True


@router.get("/get-user/{user_id}", response_model=UserSchema)
async def get_user(user_id: str, db: AsyncSession = Depends(get_db)):
    user = await UserModel.get_user(db, user_id)
    return user


@router.get("/get-users", response_model=list[UserSchema])
async def get_users(db: AsyncSession = Depends(get_db)):
    users = await UserModel.get_all_users(db)
    return users


@router.post("/create-user", response_model=UserSchema)
async def create_user(user: UserSchemaCreate, db: AsyncSession = Depends(get_db)):
    user = await UserModel.create_user(db, **user.dict())
    return user
