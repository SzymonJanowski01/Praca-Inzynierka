from fastapi import APIRouter, Depends, HTTPException, status
from fastapi.responses import JSONResponse
from fastapi.encoders import jsonable_encoder
from pydantic import BaseModel
from typing import List, Dict, Union
from sqlalchemy.ext.asyncio import AsyncSession
import numpy as np
import json

from app.services.database import get_db

from ..models import Phase as PhaseModel

router = APIRouter(prefix="/phase", tags=["phase"])


class PhaseSchemaBase(BaseModel):
    scenario_id: str
    main_character: str
    first_alternative_character: str
    second_alternative_character: str


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

    phases_dict = jsonable_encoder(phases)

    return JSONResponse(status_code=201, content=phases_dict)


@router.post("/recommendations/{user_id}")
async def get_recommendations(user_id: str, champions_list: list[str], target_position: str,
                             db: AsyncSession = Depends(get_db)):
    recommendations = await PhaseModel.get_recommendations(db, user_id, champions_list, target_position)

    if not recommendations:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND,
                            detail="No recommendation for the user")

    converted_recommendations = {region: [int(value) for value in values] for region, values in recommendations.items()}
    final_recommendations = jsonable_encoder(converted_recommendations)

    return JSONResponse(status_code=200, content=final_recommendations)


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
