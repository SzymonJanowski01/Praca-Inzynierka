from ..main import get_recommendations
from ..data_constants import POSITIONS, CHAMPIONS_IDS

import random

def test_set_values():
    example_user_input = [150, 526, 42, 429, 0, 58, 78, 163, 110, 81]
    example_target_position = "Blue_Champion_5"

    recommendations = get_recommendations(example_user_input, example_target_position)
    for region, region_recommendations in recommendations.items():
        assert len(region_recommendations) == 3
        assert all(recommendation >= 0 for recommendation in region_recommendations)

def test_random_values():
    for x in range(5):
        example_user_input = random.choices(CHAMPIONS_IDS, k=10)
        example_target_position = random.choice(POSITIONS)

        recommendations = get_recommendations(example_user_input, example_target_position)
        for region, region_recommendations in recommendations.items():
            assert len(region_recommendations) == 3
            assert all(recommendation >= 0 for recommendation in region_recommendations)

def test_zeroes_values():
    for x in range(5):
        example_user_input = [0] * 10
        example_target_position = random.choice(POSITIONS)

        recommendations = get_recommendations(example_user_input, example_target_position)
        for region, region_recommendations in recommendations.items():
            assert len(region_recommendations) == 3
            assert all(recommendation >= 0 for recommendation in region_recommendations)

def test_zeroes_with_set_target():
    example_user_input = [0] * 10
    example_target_position = random.choice(POSITIONS)
    example_user_input[POSITIONS.index(example_target_position)] = random.choice(CHAMPIONS_IDS)

    recommendations = get_recommendations(example_user_input, example_target_position)
    for region, region_recommendations in recommendations.items():
        assert len(region_recommendations) == 3
        assert all(recommendation >= 0 for recommendation in region_recommendations)

def test_zeroes_with_random_target():
    for x in range(5):
        example_user_input = [0] * 10
        example_target_position = random.choice(POSITIONS)
        example_user_input[POSITIONS.index(example_target_position)] = random.choice(CHAMPIONS_IDS)
        example_user_input[random.choice(range(10))] = random.choice(CHAMPIONS_IDS)

        recommendations = get_recommendations(example_user_input, example_target_position)
        for region, region_recommendations in recommendations.items():
            assert len(region_recommendations) == 3
            assert all(recommendation >= 0 for recommendation in region_recommendations)