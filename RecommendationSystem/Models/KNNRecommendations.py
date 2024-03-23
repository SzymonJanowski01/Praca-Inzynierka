from collections import Counter
from datetime import timedelta
import ast
import warnings
import time

import numpy as np
import pandas as pd
from sklearn.exceptions import DataConversionWarning
from sklearn.neighbors import KNeighborsClassifier
from sklearn.model_selection import train_test_split
from icecream import ic

from data_constants import DATA_EU, DATA_NA, DATA_KR, DATA_CN, POSITIONS, CHAMPIONS_IDS, BANS_CN, BANS_EU, BANS_KR, \
    BANS_NA


def encode_data(data: pd.DataFrame) -> pd.DataFrame:
    champions_list = []
    for _, row in data.iterrows():
        row_list = []
        for champ_id in CHAMPIONS_IDS:
            if champ_id in row.values:
                row_list.append(1)
            else:
                row_list.append(0)
        champions_list.append(row_list)
    encoded_data = pd.DataFrame(champions_list, columns=CHAMPIONS_IDS)
    # encoded_data.to_csv("encoded_data.csv", index=False)
    return encoded_data


def extract_target_for_each_row(data: pd.DataFrame, encoded_data: pd.DataFrame, target_position: str) -> pd.DataFrame:
    encoded_data_copy = encoded_data.copy()

    encoded_data_copy.reset_index(drop=True, inplace=True)
    encoded_data.reset_index(drop=True, inplace=True)
    data.reset_index(drop=True, inplace=True)

    encoded_data_copy['target'] = None
    for i, row in encoded_data_copy.iterrows():
        original_value = data.loc[i, target_position]
        encoded_data_copy.at[i, 'target'] = str(original_value)
        encoded_data_copy.at[i, original_value] = 0
    return encoded_data_copy


class KNNRecommendation:
    def __init__(self, data: pd.DataFrame, k: int = 5, metric: str = 'minkowski', weights: str = 'uniform',
                 algorithm: str = 'auto'):
        self.data = data
        self.encoded_data = encode_data(data)
        self.k = k
        self.weights = weights
        self.metric = metric
        self.algorithm = algorithm
        self.models = {position: KNeighborsClassifier(n_neighbors=self.k, metric=self.metric, weights=self.weights,
                                                      algorithm=self.algorithm) for position in POSITIONS}

    def test(self, bans: pd.DataFrame) -> float:
        all_accuracies = []

        for position, model in self.models.items():
            prepared_data = extract_target_for_each_row(self.data, self.encoded_data, position)

            x = prepared_data.drop(columns=["target"])
            y = prepared_data["target"]

            x_train, x_test, y_train, y_test = train_test_split(x, y, test_size=0.2, random_state=42)

            model.fit(x_train, y_train)

            correct_predictions = 0
            total_predictions = 0

            for i, x_test_instance in enumerate(x_test.values):
                true_target = y_test.iloc[i]

                banned_champions = ast.literal_eval(bans.iloc[i])
                y_pred = self.recommend(x_test_instance, position)
                y_pred.extend(banned_champions)

                if int(true_target) in y_pred:
                    correct_predictions += 1
                total_predictions += 1

            accuracy = (correct_predictions / total_predictions) * 100
            all_accuracies.append(accuracy)

        total_accuracy = sum(all_accuracies) / len(all_accuracies)
        total_accuracy = round(total_accuracy, 2)
        ic(f"KNN accuracy: {total_accuracy}%")

        return total_accuracy

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

        unique_recommendations = set()
        attempts = 0

        for i, neighbor_index in enumerate(neighbors_indices):
            neighbor_labels = np.array([self.data.iloc[neighbor_index][target_position]])
            recommendation = Counter(neighbor_labels).most_common(1)[0][0]

            while recommendation in unique_recommendations and attempts < len(neighbors_indices):
                i = (i + 1) % len(neighbors_indices)
                neighbor_labels = np.array([self.data.iloc[neighbors_indices[i]][target_position]])
                recommendation = Counter(neighbor_labels).most_common(1)[0][0]
                attempts += 1

            unique_recommendations.add(recommendation)

            if len(unique_recommendations) == 3:
                break

        final_recommendations = list(unique_recommendations)
        while len(final_recommendations) < 3:
            final_recommendations.append(0)

        return final_recommendations


def get_knn_models() -> dict:
    knn_regions = {}

    knn_eu = KNNRecommendation(DATA_EU, k=18, metric='chebyshev', weights='uniform', algorithm='ball_tree')
    knn_eu.fit()
    knn_regions['eu'] = knn_eu

    knn_na = KNNRecommendation(DATA_NA, k=6, metric='minkowski', weights='uniform', algorithm='ball_tree')
    knn_na.fit()
    knn_regions['na'] = knn_na

    knn_kr = KNNRecommendation(DATA_KR, k=5, metric='chebyshev', weights='uniform', algorithm='ball_tree')
    knn_kr.fit()
    knn_regions['kr'] = knn_kr

    knn_cn = KNNRecommendation(DATA_CN, k=5, metric='chebyshev', weights='uniform', algorithm='brute')
    knn_cn.fit()
    knn_regions['cn'] = knn_cn

    return knn_regions


def test_knn_models():
    print("Testing:")

    knn_eu = KNNRecommendation(DATA_EU, k=18, metric='chebyshev', weights='uniform', algorithm='ball_tree')
    print("EU")
    knn_eu.test(BANS_EU)

    knn_na = KNNRecommendation(DATA_NA, k=6, metric='minkowski', weights='uniform', algorithm='ball_tree')
    print("NA")
    knn_na.test(BANS_NA)

    knn_kr = KNNRecommendation(DATA_KR, k=5, metric='chebyshev', weights='uniform', algorithm='ball_tree')
    print("KR")
    knn_kr.test(BANS_KR)

    knn_cn = KNNRecommendation(DATA_CN, k=5, metric='chebyshev', weights='uniform', algorithm='brute')
    print("CN")
    knn_cn.test(BANS_CN)


def pick_best_knn_settings():
    best_accuracy_eu = 0
    best_k_eu = 0
    best_metric_eu = ""
    best_algorithm_eu = ""
    best_accuracy_na = 0
    best_k_na = 0
    best_metric_na = ""
    best_algorithm_na = ""
    best_accuracy_kr = 0
    best_k_kr = 0
    best_metric_kr = ""
    best_algorithm_kr = ""
    best_accuracy_cn = 0
    best_k_cn = 0
    best_metric_cn = ""
    best_algorithm_cn = ""

    warnings.filterwarnings("ignore", category=DataConversionWarning)

    start = time.perf_counter()
    for algorithm in ['ball_tree', 'kd_tree', 'brute']:
        for k in range(5, 20):
            for metric in ['minkowski', 'euclidean', 'manhattan', 'cosine', 'canberra', 'braycurtis', 'chebyshev',
                           'correlation', 'cityblock', 'sqeuclidean', 'jaccard', 'dice', 'hamming', 'rogerstanimoto',
                           'russellrao', 'sokalmichener', 'sokalsneath', 'yule']:
                ic(k, metric, algorithm)
                try:
                    ic.disable()
                    knn_cn = KNNRecommendation(DATA_CN, k, metric, 'uniform', algorithm)
                    knn_cn.fit()

                    accuracy_cn = knn_cn.test(BANS_CN)
                    if accuracy_cn > best_accuracy_cn:
                        best_accuracy_cn = accuracy_cn
                        best_k_cn = k
                        best_metric_cn = metric
                        best_algorithm_cn = algorithm

                    knn_eu = KNNRecommendation(DATA_EU, k, metric, 'uniform', algorithm)
                    knn_eu.fit()
                    accuracy_eu = knn_eu.test(BANS_EU)
                    if accuracy_eu > best_accuracy_eu:
                        best_accuracy_eu = accuracy_eu
                        best_k_eu = k
                        best_metric_eu = metric
                        best_algorithm_eu = algorithm

                    knn_na = KNNRecommendation(DATA_NA, k, metric, 'uniform', algorithm)
                    knn_na.fit()
                    accuracy_na = knn_na.test(BANS_NA)
                    if accuracy_na > best_accuracy_na:
                        best_accuracy_na = accuracy_na
                        best_k_na = k
                        best_metric_na = metric
                        best_algorithm_na = algorithm

                    knn_kr = KNNRecommendation(DATA_KR, k, metric, 'uniform', algorithm)
                    knn_kr.fit()
                    accuracy_kr = knn_kr.test(BANS_KR)
                    if accuracy_kr > best_accuracy_kr:
                        best_accuracy_kr = accuracy_kr
                        best_k_kr = k
                        best_metric_kr = metric
                        best_algorithm_kr = algorithm

                    ic.enable()
                except ValueError:
                    ic.enable()
                    continue

    best_accuracy_cn = "{:.2f}".format(best_accuracy_cn)
    best_accuracy_eu = "{:.2f}".format(best_accuracy_eu)
    best_accuracy_na = "{:.2f}".format(best_accuracy_na)
    best_accuracy_kr = "{:.2f}".format(best_accuracy_kr)
    ic(best_accuracy_cn, best_k_cn, best_metric_cn, best_algorithm_cn)
    ic(best_accuracy_eu, best_k_eu, best_metric_eu, best_algorithm_eu)
    ic(best_accuracy_na, best_k_na, best_metric_na, best_algorithm_na)
    ic(best_accuracy_kr, best_k_kr, best_metric_kr, best_algorithm_kr)

    end = time.perf_counter()
    final = end - start
    ic(f"Time: {timedelta(seconds=final)}")


if __name__ == "__main__":
    pick_best_knn_settings()
