
class Config:
    user = ""
    password = ""
    host = ""
    name = ""

    DB_CONFIG = f"postgres+asyncpg://{user}:{password}@{host}/{name}"


config = Config
