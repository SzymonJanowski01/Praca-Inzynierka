from collections import Counter

import numpy as np
import pandas as pd
from sklearn.neighbors import KNeighborsClassifier
from sklearn.model_selection import train_test_split
from sklearn.metrics import accuracy_score
from icecream import ic

from data_constants import data_eu, data_na, data_kr, data_cn, positions, champions_ids


def encode_data(data: pd.DataFrame) -> pd.DataFrame:
    champions_list = []
    for _, row in data.iterrows():
        row_list = []
        for champ_id in champions_ids:
            if champ_id in row.values:
                row_list.append(1)
            else:
                row_list.append(0)
        champions_list.append(row_list)
    encoded_data = pd.DataFrame(champions_list, columns=champions_ids)
    encoded_data.to_csv("encoded_data.csv", index=False)
    return encoded_data


def extract_target_for_each_row(data: pd.DataFrame, encoded_data: pd.DataFrame, target_position: str) -> pd.DataFrame:
    encoded_data['target'] = None
    for i, row in encoded_data.iterrows():
        original_value = data.loc[i, target_position]
        encoded_data.at[i, 'target'] = str(original_value)
        encoded_data.at[i, original_value] = 0
    return encoded_data


class KNNRecommendation:
    def __init__(self, data: pd.DataFrame, k: int = 5):
        self.models = {position: KNeighborsClassifier(n_neighbors=k) for position in positions}
        self.data = data
        self.encoded_data = encode_data(data)
        self.k = k

    # TODO: change the way testing is done as this is not good
    def test(self):
        for position, model in self.models.items():
            prepared_data = extract_target_for_each_row(self.data, self.encoded_data, position)
            x = prepared_data.drop(columns=["target"])
            y = prepared_data["target"]

            x_train, x_test, y_train, y_test = train_test_split(x, y, test_size=0.2, random_state=42)

            model.fit(x_train, y_train)

            y_pred = model.predict(x_test)
            print(f"Accuracy for {position}: %.2f%%" % (accuracy_score(y_test, y_pred) * 100.0))
            print(f"Predicted {y_pred} vs actual {y_test}")

    def fit(self):
        for position, model in self.models.items():
            prepared_data = extract_target_for_each_row(self.data, self.encoded_data, position)
            x = prepared_data.drop(columns=["target"])
            y = prepared_data["target"]

            model.fit(x, y)

    def recommend(self, user_input: list, target_position: str) -> list:
        knn_model = self.models[target_position]

        user_input_reshaped = np.array(user_input).reshape(1, -1)

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
    knn_eu.test()
    knn_eu.fit()
    knn_regions['eu'] = knn_eu

    # knn_na = KNNRecommendation(data_na)
    # knn_na.test()
    # knn_na.fit()
    # knn_regions['na'] = knn_na
    #
    # knn_kr = KNNRecommendation(data_kr)
    # knn_kr.test()
    # knn_kr.fit()
    # knn_regions['kr'] = knn_kr
    #
    # knn_cn = KNNRecommendation(data_cn)
    # knn_cn.test()
    # knn_cn.fit()
    # knn_regions['cn'] = knn_cn

    return knn_regions
