from fastapi import APIRouter, Depends, HTTPException, status
from pydantic import BaseModel
from sqlalchemy.ext.asyncio import AsyncSession

from app.services.database import get_db

from ..models import Scenario as ScenarioModel
from ..models import Phase as PhaseModel

router = APIRouter(prefix="/scenario", tags=["scenario"])


class ScenarioSchemaBase(BaseModel):
    name: str
    user_id: str


class ScenarioSchemaCreate(ScenarioSchemaBase):
    pass


class ScenarioSchema(ScenarioSchemaBase):
    scenario_id: str

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


@router.post("/create-scenario", response_model=ScenarioSchema)
async def create_scenario(scenario: ScenarioSchemaCreate, db: AsyncSession = Depends(get_db)):
    scenario = await ScenarioModel.create_scenario(db, scenario.user_id, scenario.name)

    await PhaseModel.create_empty_phases(db, scenario_id=scenario.id)

    return scenario


@router.put("/update_scenario/{scenario_id}", response_model=ScenarioSchema)
async def update_scenario(scenario_id: str, updated_name: str, db: AsyncSession = Depends(get_db)):
    updated_scenario = await ScenarioModel.update_scenario(db, scenario_id, updated_name)

    if not updated_scenario:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="No such scenario.")

    return updated_scenario


@router.delete("/delete_scenario/{scenario_id}")
async def delete_scenario(scenario_id: str, db: AsyncSession = Depends(get_db)):
    deleted = await ScenarioModel.delete_scenario(db, scenario_id)

    if not deleted:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="No such scenario.")

    return True
