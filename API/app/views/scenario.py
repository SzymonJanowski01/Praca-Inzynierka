from fastapi import APIRouter, Depends, HTTPException, status
from pydantic import BaseModel
from sqlalchemy.ext.asyncio import AsyncSession
from datetime import datetime

from app.services.database import get_db
from app.supporting_functions import convert_to_optional

from ..models import Scenario as ScenarioModel
from ..models import Phase as PhaseModel

router = APIRouter(prefix="/scenario", tags=["scenario"])


class ScenarioSchemaBase(BaseModel):
    name: str


class ScenarioSchemaCreate(ScenarioSchemaBase):
    pass


class ScenarioSchemaUpdate(ScenarioSchemaBase):
    __annotations__ = convert_to_optional(ScenarioSchemaBase)


class ScenarioSchema(ScenarioSchemaBase):
    scenario_id: str
    user_id: str
    created_at: datetime
    last_modified_at: datetime

    class Config:
        orm_mode = True


@router.get("/get-scenario/{scenario_id}", response_model=ScenarioSchema)
async def get_scenario(scenario_id: str, db: AsyncSession = Depends(get_db)):
    scenario = await ScenarioModel.get_scenario(db, scenario_id)

    if not scenario:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="No such scenario.")

    return scenario


@router.get("/get-user-scenarios/{user_id}", response_model=list[ScenarioSchema])
async def get_user_scenarios(user_id: str, db: AsyncSession = Depends(get_db)):
    scenarios = await ScenarioModel.get_all_user_scenarios(db, user_id)

    if not scenarios:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="No scenarios associated with the user.")

    return scenarios


@router.post("/create-scenario/{user_id}", response_model=ScenarioSchema)
async def create_scenario(user_id: str, scenario: ScenarioSchemaCreate, db: AsyncSession = Depends(get_db)):
    scenario = await ScenarioModel.create_scenario(db, user_id, scenario.name)

    await PhaseModel.create_empty_phases(db, scenario.scenario_id)

    return scenario


@router.put("/update_scenario/{scenario_id}", response_model=ScenarioSchema)
async def update_scenario(scenario_id: str, scenario: ScenarioSchemaUpdate, db: AsyncSession = Depends(get_db)):
    updated_scenario = await ScenarioModel.update_scenario(db, scenario_id, scenario.name)

    if not updated_scenario:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="No such scenario.")

    return updated_scenario


@router.delete("/delete_scenario/{scenario_id}")
async def delete_scenario(scenario_id: str, db: AsyncSession = Depends(get_db)):
    deleted = await ScenarioModel.delete_scenario(db, scenario_id)

    if not deleted:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="No such scenario.")

    return True
