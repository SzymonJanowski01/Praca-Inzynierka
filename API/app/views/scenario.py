from fastapi import APIRouter, Depends
from pydantic import BaseModel
from sqlalchemy.ext.asyncio import AsyncSession

from app.services.database import get_db

from ..models import Scenario as ScenarioModel

router = APIRouter(prefix="/scenario", tags=["scenario"])


class ScenarioSchemaBase(BaseModel):
    user_id: str | None = None
    name: str | None = None


class ScenarioSchemaCreate(ScenarioSchemaBase):
    pass


class ScenarioSchema(ScenarioSchemaBase):
    id: str

    class Config:
        orm_mode = True


@router.get("/get-scenario", response_model=ScenarioSchema)
async def get_scenario(id: str, db: AsyncSession = Depends(get_db)):
    scenario = await ScenarioModel.get(db, id)
    return scenario


@router.get("/get-user-scenarios", response_model=list[ScenarioSchema])
async def get_users(user_id: str, db: AsyncSession = Depends(get_db)):
    scenarios = await ScenarioModel.get_all(db, user_id)
    return scenarios


@router.post("/create-scenario", response_model=ScenarioSchema)
async def create_user(scenario: ScenarioSchemaCreate, db: AsyncSession = Depends(get_db)):
    scenario = await ScenarioModel.create(db, **scenario.dict())
    return scenario
