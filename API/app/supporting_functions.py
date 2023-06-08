from typing import Optional


def convert_to_optional(schema):
    return {i: Optional[j] for i, j in schema.__annotations__.items()}
