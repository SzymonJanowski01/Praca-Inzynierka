from fastapi import APIRouter, Depends
from pydantic import BaseModel
from sqlalchemy.ext.asyncio import AsyncSession

from app.services.database import get_db
from app.supporting_functions import convert_to_optional

from ..models import Phase as PhaseModel

router = APIRouter(prefix="/phase", tags=["phase"])


class PhaseSchemaBase(BaseModel):
    name: str


class PhaseSchemaCreate(PhaseSchemaBase):
    pass


class PhaseSchemaUpdate(PhaseSchemaBase):
    __annotations__ = convert_to_optional(PhaseSchemaBase)


class PhaseSchema(PhaseSchemaBase):
    id: str
    scenario_id: str

    class Config:
        orm_mode = True


@router.get("/get-scenario-phases", response_model=list[PhaseSchema])
async def get_phases(db: AsyncSession = Depends(get_db)):
    phases = await PhaseModel.get_all(db)
    return phases


@router.post("/create-phase", response_model=PhaseSchema)
async def create_phase(phase: PhaseSchemaCreate, db: AsyncSession = Depends(get_db)):
    phase = await PhaseModel.create(db, **phase.dict())
    return phase
