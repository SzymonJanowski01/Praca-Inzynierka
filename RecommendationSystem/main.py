from Models.KNNRecommendations import get_knn_models
from Models.CosineSim import get_similarity_recommendations
from icecream import ic

from data_constants import champions_ids


def get_recommendations(user_input, target_position):
    knn_models = get_knn_models()
    knn_recommendations = {
        'eu': knn_models["eu"].recommend(user_input, target_position),
        'na': knn_models["na"].recommend(user_input, target_position),
        'kr': knn_models["kr"].recommend(user_input, target_position),
        'cn': knn_models["cn"].recommend(user_input, target_position)
    }

    similarity_recommendations = get_similarity_recommendations(user_input, target_position)
    print(f"Cosine: {similarity_recommendations}")
    print(f"KNN: {knn_recommendations}")


def create_game_scenario(user_input):
    game_scenario = [0] * len(champions_ids)
    for champ_id in user_input:
        if champ_id != 0:
            game_scenario[champions_ids.index(champ_id)] = 1
    return game_scenario


def main():
    user_input = [897,78,268,110,0,24,5,101,236,267]
    target_position = "Blue_Champion_5"

    get_recommendations(create_game_scenario(user_input), target_position)


if __name__ == "__main__":
    main()
