from collections import Counter

import numpy as np
import pandas as pd
from sklearn.neighbors import KNeighborsClassifier

from data_constants import data_eu, data_na, data_kr, data_cn, positions


class KNNRecommendation:
    def __init__(self, data: pd.DataFrame, k: int = 5):
        self.models = {position: KNeighborsClassifier(n_neighbors=k) for position in positions}
        self.data = data
        self.k = k

    def fit(self):
        for position, model in self.models.items():
            x = self.data.drop(columns=[position])
            y = self.data[position]

            model.fit(x.values, y.values)

    def recommend(self, user_input: list, target_position: str) -> list:
        knn_model = self.models[target_position]
        target_index = positions.index(target_position)

        user_input_reshaped = np.array(user_input[:target_index] + user_input[target_index + 1:]).reshape(1, -1)

        neighbors_indices = knn_model.kneighbors(user_input_reshaped, return_distance=False)[0]

        recommendations = []
        unique_recommendations = set()
        for i, neighbor_index in enumerate(neighbors_indices):
            neighbor_labels = np.array([self.data.iloc[neighbor_index][target_position]])
            recommendation = Counter(neighbor_labels).most_common(1)[0][0]

            while recommendation in unique_recommendations:
                i = (i + 1) % len(neighbors_indices)
                neighbor_labels = np.array([self.data.iloc[neighbors_indices[i]][target_position]])
                recommendation = Counter(neighbor_labels).most_common(1)[0][0]

            unique_recommendations.add(recommendation)
            recommendations.append(recommendation)

            if len(unique_recommendations) == 3:
                break

        return recommendations


def get_knn_models() -> dict:
    knn_regions = {}

    knn_eu = KNNRecommendation(data_eu)
    knn_eu.fit()
    knn_regions['eu'] = knn_eu

    knn_na = KNNRecommendation(data_na)
    knn_na.fit()
    knn_regions['na'] = knn_na

    knn_kr = KNNRecommendation(data_kr)
    knn_kr.fit()
    knn_regions['kr'] = knn_kr

    knn_cn = KNNRecommendation(data_cn)
    knn_cn.fit()
    knn_regions['cn'] = knn_cn

    return knn_regions
