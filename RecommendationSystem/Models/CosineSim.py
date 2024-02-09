from sklearn.metrics.pairwise import cosine_similarity
import numpy as np

from data_constants import data_eu, data_na, data_kr, data_cn, positions


def get_similarity_recommendations(base_list: list, target_position: str) -> dict:
    cos_sim_regions = {'eu': [], 'na': [], 'kr': [], 'cn': []}

    for region_data, region_name in zip([data_eu, data_na, data_kr, data_cn], ['eu', 'na', 'kr', 'cn']):
        similarities = []

        base_vector = np.isin(base_list, base_list).astype(int)

        for index, row in region_data.reset_index(drop=True).iterrows():
            vector = np.isin(base_list, row).astype(int)

            similarity = cosine_similarity([base_vector], [vector])[0, 0]
            similarities.append((index, similarity))

        similarities.sort(key=lambda x: x[1], reverse=True)

        target_index = positions.index(target_position)

        top_recommendations = []
        unique_recommendations = set()
        for index, value in similarities:
            recommendation = region_data.iloc[index].iloc[target_index]

            if recommendation not in unique_recommendations:
                top_recommendations.append(recommendation)
                unique_recommendations.add(recommendation)

            if len(unique_recommendations) == 3:
                break

        cos_sim_regions[region_name] = top_recommendations

    return cos_sim_regions


