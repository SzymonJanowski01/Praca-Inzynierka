from sklearn.metrics.pairwise import cosine_similarity
import numpy as np

from data_constants import DATA_EU, DATA_NA, DATA_KR, DATA_CN, POSITIONS


def get_similarity_recommendations(base_list: list, target_position: str) -> dict:
    cos_sim_regions = {'eu': [], 'na': [], 'kr': [], 'cn': []}

    for region_data, region_name in zip([DATA_EU, DATA_NA, DATA_KR, DATA_CN], ['eu', 'na', 'kr', 'cn']):
        similarities = []

        base_vector = np.isin(base_list, base_list).astype(int)

        for index, row in region_data.reset_index(drop=True).iterrows():
            vector = np.isin(base_list, row).astype(int)

            similarity = cosine_similarity([base_vector], [vector])[0, 0]
            similarities.append((index, similarity))

        similarities.sort(key=lambda x: x[1], reverse=True)

        target_index = POSITIONS.index(target_position)

        unique_recommendations = set()
        for index, value in similarities:
            recommendation = region_data.iloc[index].iloc[target_index]

            if recommendation not in unique_recommendations:
                unique_recommendations.add(recommendation)

            if len(unique_recommendations) == 3:
                break

        cos_sim_regions[region_name] = list(unique_recommendations)

    return cos_sim_regions


