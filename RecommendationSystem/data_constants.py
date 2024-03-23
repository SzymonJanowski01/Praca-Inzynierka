import pandas as pd
from icecream import ic
from typing import List

DATA: pd.DataFrame = pd.read_csv(r'D:\GitHubDesktop - new\Praca-Inzynierka\DataGathering\most_recent_games.csv')

POSITIONS: List[str] = ['Blue_Champion_1', 'Blue_Champion_2', 'Blue_Champion_3', 'Blue_Champion_4', 'Blue_Champion_5',
                        'Red_Champion_1', 'Red_Champion_2', 'Red_Champion_3', 'Red_Champion_4', 'Red_Champion_5']

CHAMPIONS_IDS: List[int] = [1, 10, 101, 102, 103, 104, 105, 106, 107, 11, 110, 111, 112, 113, 114, 115, 117, 119, 12,
                            120, 121, 122, 126, 127, 13, 131, 133, 134, 136, 14, 141, 142, 143, 145, 147, 15, 150, 154,
                            157, 16, 161, 163, 164, 166, 17, 18, 19, 2, 20, 200, 201, 202, 203, 21, 22, 221, 222, 223,
                            23, 233, 234, 235, 236, 238, 24, 240, 245, 246, 25, 254, 26, 266, 267, 268, 27, 28, 29, 3,
                            30, 31, 32, 33, 34, 35, 350, 36, 360, 37, 38, 39, 4, 40, 41, 412, 42, 420, 421, 427, 429,
                            43, 432, 44, 45, 48, 497, 498, 5, 50, 51, 516, 517, 518, 523, 526, 53, 54, 55, 555, 56, 57,
                            58, 59, 6, 60, 61, 62, 63, 64, 67, 68, 69, 7, 711, 72, 74, 75, 76, 77, 777, 78, 79, 8, 80,
                            81, 82, 83, 84, 85, 86, 875, 876, 887, 888, 89, 895, 897, 9, 90, 901, 902, 91, 910, 92, 950,
                            96, 98, 99]

bans_eu = DATA[DATA['Region'] == 'LEC'].drop(columns=['Region'])
BANS_EU = bans_eu['Bans']
bans_na = DATA[DATA['Region'] == 'LCS'].drop(columns=['Region'])
BANS_NA = bans_na['Bans']
bans_kr = DATA[DATA['Region'] == 'LCK'].drop(columns=['Region'])
BANS_KR = bans_kr['Bans']
bans_cn = DATA[DATA['Region'] == 'LPL'].drop(columns=['Region'])
BANS_CN = bans_cn['Bans']

DATA_EU = DATA[DATA['Region'] == 'LEC'].drop(columns=['Region', 'Bans'])
DATA_NA = DATA[DATA['Region'] == 'LCS'].drop(columns=['Region', 'Bans'])
DATA_KR = DATA[DATA['Region'] == 'LCK'].drop(columns=['Region', 'Bans'])
DATA_CN = DATA[DATA['Region'] == 'LPL'].drop(columns=['Region', 'Bans'])

if __name__ == "__main__":
    ic(len(DATA_CN))
    ic(len(DATA_EU))
    ic(len(DATA_NA))
    ic(len(DATA_KR))

    # DATA_CN.to_csv("data_cn.csv", index=False)
    # BANS_CN.to_csv("bans_cn.csv", index=False)
