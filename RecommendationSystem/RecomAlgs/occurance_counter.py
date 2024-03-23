from data_constants import DATA_EU, DATA_NA, DATA_KR, DATA_CN, POSITIONS


def get_most_common_champions(target_postion: str) -> dict[str, list[int]]:
    most_common_champions = {'eu': [], 'na': [], 'kr': [], 'cn': []}

    for region_data, region_name in zip([DATA_EU, DATA_NA, DATA_KR, DATA_CN], ['eu', 'na', 'kr', 'cn']):
        index_of_target_position = POSITIONS.index(target_postion)

        most_common_champions[region_name] = region_data.iloc[:, index_of_target_position].value_counts().index[:3].tolist()

        while len(most_common_champions[region_name]) < 3:
            most_common_champions[region_name].append(0)

    return most_common_champions
