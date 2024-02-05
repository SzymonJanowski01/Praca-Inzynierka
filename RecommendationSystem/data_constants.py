import pandas as pd

data = pd.read_csv(r'D:\GitHubDesktop - new\Praca-Inzynierka\DataGathering\most_recent_games.csv')

data_eu = data[data['Region'] == 'LEC'].drop(columns=['Region'])
data_na = data[data['Region'] == 'LCS'].drop(columns=['Region'])
data_kr = data[data['Region'] == 'LCK'].drop(columns=['Region'])
data_cn = data[data['Region'] == 'LPL'].drop(columns=['Region'])


positions = ['Blue_Champion_1', 'Blue_Champion_2', 'Blue_Champion_3', 'Blue_Champion_4', 'Blue_Champion_5',
             'Red_Champion_1', 'Red_Champion_2', 'Red_Champion_3', 'Red_Champion_4', 'Red_Champion_5']
