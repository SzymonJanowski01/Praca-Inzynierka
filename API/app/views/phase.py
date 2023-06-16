from fastapi import APIRouter, Depends, HTTPException, status
from pydantic import BaseModel
from typing import List, Dict, Union
from sqlalchemy.ext.asyncio import AsyncSession

from app.services.database import get_db
from app.supporting_functions import convert_to_optional

from ..models import Phase as PhaseModel

router = APIRouter(prefix="/phase", tags=["phase"])


class PhaseSchemaBase(BaseModel):
    scenario_id: str
    main_character: str
    firs_alternative_character: str
    second_alternative_character: str


class PhaseSchemaCreate(PhaseSchemaBase):
    pass


class PhaseSchemaUpdate(PhaseSchemaBase):
    __annotations__ = convert_to_optional(PhaseSchemaBase)


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


@router.put("/update-scenario/{scenario_id}")
async def update_scenario(scenario_id: str, phases_changes: List[Dict[str, Union[int, Dict[str, str]]]],
                          db: AsyncSession = Depends(get_db)):
    try:
        updated_phases = await PhaseModel.update_scenario_phases(db, scenario_id, phases_changes)

        if not updated_phases:
            return HTTPException(status_code=status.HTTP_404_NOT_FOUND,
                                 detail="No such phases or scenario does not exist")

        return updated_phases
    except ValueError as e:
        raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail=str(e))
