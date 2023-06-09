from fastapi import APIRouter, Depends
from pydantic import BaseModel
from sqlalchemy.ext.asyncio import AsyncSession

from app.services.database import get_db
from app.supporting_functions import convert_to_optional

from ..models import Phase as PhaseModel

router = APIRouter(prefix="/phase", tags=["phase"])


class PhaseSchemaBase(BaseModel):
    main_character: str
    firs_alternative_character: str
    second_alternative_character: str


class PhaseSchemaCreate(PhaseSchemaBase):
    pass


class PhaseSchemaUpdate(PhaseSchemaBase):
    __annotations__ = convert_to_optional(PhaseSchemaBase)


class PhaseSchema(PhaseSchemaBase):
    id: str
    scenario_id: str
    name: str

    class Config:
        orm_mode = True


@router.get("/get-scenario-phases/{scenario_id}", response_model=list[PhaseSchema])
async def get_phases(scenario_id: str, db: AsyncSession = Depends(get_db)):
    phases = await PhaseModel.get_all_scenario_phases(db, scenario_id)
    return phases


@router.post("/create-phase", response_model=PhaseSchema)
async def create_phase(phase: PhaseSchemaCreate, db: AsyncSession = Depends(get_db)):
    phase = await PhaseModel.create_phase(db, **phase.dict())
    return phase
