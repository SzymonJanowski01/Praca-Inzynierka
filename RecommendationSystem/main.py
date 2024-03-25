import random

from .RecomAlgs.KNNRecommendations import get_knn_for_each_region, test_knn
from .RecomAlgs.CosineSim import get_similarity_recommendations
from .RecomAlgs.occurance_counter import get_most_common_champions

from .data_constants import CHAMPIONS_IDS, POSITIONS


def get_recommendations(user_input: list[int], target_position: str) -> dict[str, list[int]]:
    if all(x == 0 for x in user_input):
        recommendation = get_most_common_champions(target_position)
        return recommendation

    elif user_input.count(0) == len(user_input) - 1:
        index_of_target_position = POSITIONS.index(target_position)

        if user_input[index_of_target_position] != 0:
            recommendation = get_most_common_champions(target_position)
            return recommendation

    game_state = create_game_scenario(user_input)

    knn_from_region = get_knn_for_each_region()
    knn_recommendations = {
        'eu': knn_from_region["eu"].recommend(game_state, target_position),
        'na': knn_from_region["na"].recommend(game_state, target_position),
        'kr': knn_from_region["kr"].recommend(game_state, target_position),
        'cn': knn_from_region["cn"].recommend(game_state, target_position)
    }

    cos_recommendations = get_similarity_recommendations(user_input, target_position)

    final_recommendations = {'eu': [], 'na': [], 'kr': [], 'cn': []}

    for region in ['eu', 'na', 'kr', 'cn']:
        for recommendation in knn_recommendations[region]:
            if recommendation == 0:
                continue
            elif recommendation in cos_recommendations[region]:
                final_recommendations[region].append(recommendation)

        for recommendation in knn_recommendations[region]:
            if len(final_recommendations[region]) == 3:
                break
            if recommendation == 0:
                continue
            elif recommendation not in final_recommendations[region]:
                final_recommendations[region].append(recommendation)

        for recommendation in cos_recommendations[region]:
            if len(final_recommendations[region]) == 3:
                break
            if recommendation == 0:
                continue
            elif recommendation not in final_recommendations[region]:
                final_recommendations[region].append(recommendation)
        while len(final_recommendations[region]) < 3:
            final_recommendations[region].append(0)

    return final_recommendations


def create_game_scenario(user_input: list[int]) -> list[int]:
    game_scenario = [0] * len(CHAMPIONS_IDS)
    for champ_id in user_input:
        if champ_id != 0:
            game_scenario[CHAMPIONS_IDS.index(champ_id)] = 1
    return game_scenario


def main() -> None:
    test_knn()


if __name__ == "__main__":
    main()
