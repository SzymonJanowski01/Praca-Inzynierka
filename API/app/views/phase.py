from fastapi import APIRouter, Depends, HTTPException, status
from pydantic import BaseModel
from typing import List, Dict, Union
from sqlalchemy.ext.asyncio import AsyncSession

from app.services.database import get_db

from ..models import Phase as PhaseModel

router = APIRouter(prefix="/phase", tags=["phase"])


class PhaseSchemaBase(BaseModel):
    scenario_id: str
    main_character: str
    first_alternative_character: str | None = None
    second_alternative_character: str | None = None


class PhaseSchema(PhaseSchemaBase):
    phase_id: str
    name: str

    class Config:
        orm_mode = True


@router.get("/get-scenario-phases/{scenario_id}", response_model=list[PhaseSchema])
async def get_phases(scenario_id: str, db: AsyncSession = Depends(get_db)):
    phases = await PhaseModel.get_all_scenario_phases(db, scenario_id)

    if not phases:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND,
                            detail="No phases associated with provided scenario_id")

    return phases


@router.post("/create-empty-phases/{scenario_id}")
async def create_empty_phases(scenario_id: str, db: AsyncSession = Depends(get_db)):
    phases = await PhaseModel.create_empty_phases(db, scenario_id)

    return phases


@router.put("/update-scenario-phases/{scenario_id}")
async def update_scenario_phases(scenario_id: str, phases_changes: List[Dict[str, Union[int, Dict[str, str]]]],
                          db: AsyncSession = Depends(get_db)):
    try:
        updated_phases = await PhaseModel.update_scenario_phases(db, scenario_id, phases_changes)

        if not updated_phases:
            raise HTTPException(status_code=status.HTTP_404_NOT_FOUND,
                                detail="No such phases or scenario does not exist")

        return True
    except ValueError as e:
        raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail=str(e))
