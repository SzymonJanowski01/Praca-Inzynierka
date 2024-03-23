from RecomAlgs.KNNRecommendations import get_knn_for_each_region, test_knn
from RecomAlgs.CosineSim import get_similarity_recommendations
from RecomAlgs.occurance_counter import get_most_common_champions

from data_constants import CHAMPIONS_IDS, POSITIONS


def get_recommendations(user_input: list[int], target_position: str) -> dict[str, list[int]]:
    if all(x == 0 for x in user_input):
        recommendation = get_most_common_champions(target_position)
        print(f"Occurance recommendation: {recommendation}")
        return recommendation

    elif user_input.count(0) == len(user_input) - 1:
        index_of_target_position = POSITIONS.index(target_position)

        if user_input[index_of_target_position] != 0:
            recommendation = get_most_common_champions(target_position)
            print(f"Occurance recommendation: {recommendation}")
            return recommendation

    game_state = create_game_scenario(user_input)

    knn_models = get_knn_for_each_region()
    knn_recommendations = {
        'eu': knn_models["eu"].recommend(game_state, target_position),
        'na': knn_models["na"].recommend(game_state, target_position),
        'kr': knn_models["kr"].recommend(game_state, target_position),
        'cn': knn_models["cn"].recommend(game_state, target_position)
    }

    cos_recommendations = get_similarity_recommendations(user_input, target_position)

    print(f"Cos: {cos_recommendations}")
    print(f"KNN: {knn_recommendations}")

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

    print(f"Final: {final_recommendations}")
    return final_recommendations


def create_game_scenario(user_input: list[int]) -> list[int]:
    game_scenario = [0] * len(CHAMPIONS_IDS)
    for champ_id in user_input:
        if champ_id != 0:
            game_scenario[CHAMPIONS_IDS.index(champ_id)] = 1
    return game_scenario


def main() -> None:
    testing_models = False
    if testing_models:
        test_knn()
    else:
        example_user_input = [897,78,268,110,0,24,5,101,236,267]
        example_target_position = "Blue_Champion_5"

        get_recommendations(example_user_input, example_target_position)


if __name__ == "__main__":
    main()
