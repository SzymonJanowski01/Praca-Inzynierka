from typing import Optional


def convert_to_optional(schema) -> dict:
    return {i: Optional[j] for i, j in schema.__annotations__.items()}


class Paging:
    MAX_SCENARIO_LIMIT: int = 5
    basic_limit: int = 5

    def __init__(self, skip: int = 0, limit: int = basic_limit):
        self.skip = skip
        self.limit = limit
