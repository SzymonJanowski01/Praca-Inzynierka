import pandas as pd
from icecream import ic

data = pd.read_csv(r'D:\GitHubDesktop - new\Praca-Inzynierka\DataGathering\most_recent_games.csv')

positions = ['Blue_Champion_1', 'Blue_Champion_2', 'Blue_Champion_3', 'Blue_Champion_4', 'Blue_Champion_5',
             'Red_Champion_1', 'Red_Champion_2', 'Red_Champion_3', 'Red_Champion_4', 'Red_Champion_5']

champions_ids = [1, 10, 101, 102, 103, 104, 105, 106, 107, 11, 110, 111, 112, 113, 114, 115, 117, 119, 12, 120, 121,
                 122, 126, 127, 13, 131, 133, 134, 136, 14, 141, 142, 143, 145, 147, 15, 150, 154, 157, 16, 161, 163,
                 164, 166, 17, 18, 19, 2, 20, 200, 201, 202, 203, 21, 22, 221, 222, 223, 23, 233, 234, 235, 236, 238,
                 24, 240, 245, 246, 25, 254, 26, 266, 267, 268, 27, 28, 29, 3, 30, 31, 32, 33, 34, 35, 350, 36, 360, 37,
                 38, 39, 4, 40, 41, 412, 42, 420, 421, 427, 429, 43, 432, 44, 45, 48, 497, 498, 5, 50, 51, 516, 517,
                 518, 523, 526, 53, 54, 55, 555, 56, 57, 58, 59, 6, 60, 61, 62, 63, 64, 67, 68, 69, 7, 711, 72, 74, 75,
                 76, 77, 777, 78, 79, 8, 80, 81, 82, 83, 84, 85, 86, 875, 876, 887, 888, 89, 895, 897, 9, 90, 901, 902,
                 91, 910, 92, 950, 96, 98, 99]


bans_eu = data[data['Region'] == 'LEC'].drop(columns=['Region'])
bans_eu = bans_eu['Bans']
bans_na = data[data['Region'] == 'LCS'].drop(columns=['Region'])
bans_na = bans_na['Bans']
bans_kr = data[data['Region'] == 'LCK'].drop(columns=['Region'])
bans_kr = bans_kr['Bans']
bans_cn = data[data['Region'] == 'LPL'].drop(columns=['Region'])
bans_cn = bans_cn['Bans']

data_eu = data[data['Region'] == 'LEC'].drop(columns=['Region', 'Bans'])
data_na = data[data['Region'] == 'LCS'].drop(columns=['Region', 'Bans'])
data_kr = data[data['Region'] == 'LCK'].drop(columns=['Region', 'Bans'])
data_cn = data[data['Region'] == 'LPL'].drop(columns=['Region', 'Bans'])


if __name__ == "__main__":
    ic(len(data_cn))
    ic(len(data_eu))
    ic(len(data_na))
    ic(len(data_kr))

    data_cn.to_csv("data_cn.csv", index=False)
    bans_cn.to_csv("bans_cn.csv", index=False)


