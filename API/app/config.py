
class Config:
    user = ""
    password = ""
    host = ""
    name = ""

    DB_CONFIG = f"postgresql+asyncpg://{user}:{password}@{host}/{name}"


config = Config
