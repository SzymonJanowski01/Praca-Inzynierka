from fastapi import APIRouter, Depends
from pydantic import BaseModel
from sqlalchemy.ext.asyncio import AsyncSession

from app.services.database import get_db

from ..models import Scenario as ScenarioModel
from ..models import Phase as PhaseModel

router = APIRouter(prefix="/scenario", tags=["scenario"])


class ScenarioSchemaBase(BaseModel):
    name: str


class ScenarioSchemaCreate(ScenarioSchemaBase):
    pass


class ScenarioSchema(ScenarioSchemaBase):
    id: str
    user_id: str
    phases_count: int

    class Config:
        orm_mode = True


@router.get("/get-scenario/{scenario_id}", response_model=ScenarioSchema)
async def get_scenario(scenario_id: str, db: AsyncSession = Depends(get_db)):
    scenario = await ScenarioModel.get_scenario(db, scenario_id)
    return scenario


@router.get("/get-user-scenarios/{user_id}", response_model=list[ScenarioSchema])
async def get_user_scenarios(user_id: str, db: AsyncSession = Depends(get_db)):
    scenarios = await ScenarioModel.get_all_user_scenarios(db, user_id)
    return scenarios


@router.post("/create-scenario", response_model=ScenarioSchema)
async def create_scenario(scenario: ScenarioSchemaCreate, db: AsyncSession = Depends(get_db)):
    scenario = await ScenarioModel.create_scenario(db, **scenario.dict())

    await PhaseModel.create_empty_phases(db, scenario_id=scenario.id)

    return scenario


@router.put("/update_scenario/{scenario_id}", response_model=ScenarioSchema)
async def update_scenario(scenario_id: str, updated_name: str, db: AsyncSession = Depends(get_db)):
    updated_scenario = await ScenarioModel.update_scenario(db, scenario_id, updated_name)
    return updated_scenario


@router.delete("/delete_scenario/{scenario_id}")
async def delete_scenario(scenario_id: str, db: AsyncSession = Depends(get_db)):
    await ScenarioModel.delete_scenario(db, scenario_id)
    return True
