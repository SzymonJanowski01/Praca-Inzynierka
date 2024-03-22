from Models.KNNRecommendations import get_knn_models, test_knn_models
from Models.CosineSim import get_similarity_recommendations

from data_constants import champions_ids


def get_recommendations(user_input, target_position) -> dict:
    game_state = create_game_scenario(user_input)

    knn_models = get_knn_models()
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
            if recommendation in cos_recommendations[region]:
                final_recommendations[region].append(recommendation)
        if len(final_recommendations[region]) < 3:
            for recommendation in knn_recommendations[region]:
                if recommendation not in final_recommendations[region]:
                    final_recommendations[region].append(recommendation)

    print(f"Final: {final_recommendations}")
    return final_recommendations


def create_game_scenario(user_input):
    game_scenario = [0] * len(champions_ids)
    for champ_id in user_input:
        if champ_id != 0:
            game_scenario[champions_ids.index(champ_id)] = 1
    return game_scenario


def main():
    testing_models = False
    if testing_models:
        test_knn_models()
    else:
        example_user_input = [897,78,268,110,0,24,5,101,236,267]
        example_target_position = "Blue_Champion_5"

        get_recommendations(example_user_input, example_target_position)


if __name__ == "__main__":
    main()
