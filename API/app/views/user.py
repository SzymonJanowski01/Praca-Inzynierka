from fastapi import APIRouter, Depends, HTTPException, status
from pydantic import BaseModel
from sqlalchemy.ext.asyncio import AsyncSession

from app.services.database import get_db
from app.supporting_functions import convert_to_optional

from ..models import User as UserModel

router = APIRouter(prefix="/user", tags=["user"])


class UserSchemaBase(BaseModel):
    username: str
    email: str
    password: str


class UserSchemaCreate(UserSchemaBase):
    pass


class UserSchemaUpdate(UserSchemaBase):
    __annotations__ = convert_to_optional(UserSchemaBase)


class UserSchema(UserSchemaBase):
    user_id: str
    salt: bytes

    class Config:
        orm_mode = True


@router.get("/get-user/{user_id}", response_model=UserSchema)
async def get_user(user_id: str, db: AsyncSession = Depends(get_db)):
    user = await UserModel.get_user(db, user_id)

    if not user:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="No such user.")

    return user


@router.get("/get-users", response_model=list[UserSchema])
async def get_users(db: AsyncSession = Depends(get_db)):
    users = await UserModel.get_all_users(db)

    if not users:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="No users found.")

    return users


@router.post("/create-user", response_model=UserSchema)
async def create_user(user: UserSchemaCreate, db: AsyncSession = Depends(get_db)):
    try:
        user = await UserModel.create_user(db, user.username, user.email, user.password)
        return user
    except ValueError as e:
        raise HTTPException(status_code=status.HTTP_409_CONFLICT, detail=str(e))


@router.post("/check-credentials")
async def check_credentials(username_or_email: str, provided_password: str, db: AsyncSession = Depends(get_db)):
    is_valid = await UserModel.verify_credentials(db, username_or_email, provided_password)

    if is_valid is None:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="No such user.")
    elif is_valid is False:
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Provided credentials are invalid.")

    return is_valid


@router.put("/update-user/{user_id}", response_model=UserSchema)
async def update_user(user_id: str, user: UserSchemaUpdate, db: AsyncSession = Depends(get_db)):
    try:
        user = await UserModel.update_user(db, user_id, user.username, user.email, user.password)

        if not user:
            raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="No such user.")

        return user
    except ValueError as e:
        raise HTTPException(status_code=status.HTTP_409_CONFLICT, detail=str(e))


@router.delete("/delete-user/{user_id}")
async def delete_user(user_id: str, db: AsyncSession = Depends(get_db)):
    deleted = await UserModel.delete_user(db, user_id)

    if not deleted:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="No such user.")

    return True

