from contextlib import asynccontextmanager
from fastapi import FastAPI

from app.config import config
from app.services.database import sessionmanager


def init_app(init_db=True):
    lifespan = None

    if init_db:
        sessionmanager.init(config.DB_CONFIG)

        @asynccontextmanager
        async def lifespan(app: FastAPI):
            yield
            if sessionmanager._engine is not None:
                await sessionmanager.close()

    server = FastAPI(title="FastAPI Server", lifespan=lifespan)

    from app.views.user import router as user_router
    from app.views.scenario import router as scenario_router

    server.include_router(user_router, prefix="/api", tags=["user"])
    server.include_router(scenario_router, prefix="api", tags=["scenario"])

    return server
