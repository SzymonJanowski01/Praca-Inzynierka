from fastapi import APIRouter, Depends
from pydantic import BaseModel
from sqlalchemy.ext.asyncio import AsyncSession

from app.services.database import get_db

from ..models import Scenario as ScenarioModel

router = APIRouter(prefix="/scenario", tags=["scenario"])


class ScenarioSchemaBase(BaseModel):
    name: str


class ScenarioSchemaCreate(ScenarioSchemaBase):
    pass


class ScenarioSchema(ScenarioSchemaBase):
    id: str
    user_id: str

    class Config:
        orm_mode = True


@router.get("/get-scenario", response_model=ScenarioSchema)
async def get_scenario(scenario_id: str, db: AsyncSession = Depends(get_db)):
    scenario = await ScenarioModel.get_scenario(db, scenario_id)
    return scenario


@router.get("/get-user-scenarios", response_model=list[ScenarioSchema])
async def get_user_scenarios(user_id: str, db: AsyncSession = Depends(get_db)):
    scenarios = await ScenarioModel.get_all_user_scenarios(db, user_id)
    return scenarios


@router.post("/create-scenario", response_model=ScenarioSchema)
async def create_scenario(scenario: ScenarioSchemaCreate, db: AsyncSession = Depends(get_db)):
    scenario = await ScenarioModel.create_scenario(db, **scenario.dict())
    return scenario


@router.put("/update_scenario/{scenario_id}", response_model=ScenarioSchema)
async def update_scenario(scenario_id: str, updated_name: str, db: AsyncSession = Depends(get_db)):
    updated_scenario = await ScenarioModel.update_scenario(db, scenario_id, updated_name)
    return updated_scenario
