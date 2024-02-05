import time
from typing import List
from datetime import datetime

import leaguepedia_parser as lp
import lol_id_tools as lit
import pandas as pd


def find_most_recent_tournament(regions: List[str], year: int) -> List[str]:
    """
    Finds the most recent tournament for each region in the given year.
    :param regions: List of regions to search
    :param year: Year to search
    :return: List of tournaments
    """
    print("Finding most recent tournaments", end="")
    tournaments_overview_pages = []
    current_date = str(datetime.now().date())
    for region in regions:
        tournaments = lp.get_tournaments(region, year=year)

        valid_tournaments = [t for t in tournaments if t.end is not None and t.start <= current_date]

        if valid_tournaments:
            most_recent_tournament = max(valid_tournaments, key=lambda t: t.end)
            tournaments_overview_pages.append(most_recent_tournament.overviewPage)

        print(".", end="")
        time.sleep(5)
    return tournaments_overview_pages


def get_most_recent_games(most_recent_tournaments: List[str]) -> pd.DataFrame:
    """
    Finds the most recent games for each tournament.
    :param most_recent_tournaments: List of tournaments
    :return: List of games for each region
    """
    print("\nGathering games data", end="")
    data_for_ml = []

    for region_tournament in most_recent_tournaments:
        games = lp.get_games(region_tournament)

        for game in games:
            row = {
                'Region': region_tournament[0:3],
                'Blue_Champion_1': game.teams.BLUE.players[0].championId,
                'Blue_Champion_2': game.teams.BLUE.players[1].championId,
                'Blue_Champion_3': game.teams.BLUE.players[2].championId,
                'Blue_Champion_4': game.teams.BLUE.players[3].championId,
                'Blue_Champion_5': game.teams.BLUE.players[4].championId,
                'Red_Champion_1': game.teams.RED.players[0].championId,
                'Red_Champion_2': game.teams.RED.players[1].championId,
                'Red_Champion_3': game.teams.RED.players[2].championId,
                'Red_Champion_4': game.teams.RED.players[3].championId,
                'Red_Champion_5': game.teams.RED.players[4].championId,
            }
            data_for_ml.append(row)

        print(".", end="")
    time.sleep(5)

    df = pd.DataFrame(data_for_ml)

    return df


def check_id_matching(games: pd.DataFrame):
    latest_game_lpl = games[games['Region'] == 'LPL'].iloc[-1]
    latest_game_lec = games[games['Region'] == 'LEC'].iloc[-1]
    latest_game_lck = games[games['Region'] == 'LCK'].iloc[-1]
    latest_game_lcs = games[games['Region'] == 'LCS'].iloc[-1]

    all_regions_games = [latest_game_lpl, latest_game_lec, latest_game_lck, latest_game_lcs]
    for game in all_regions_games:
        blue_champions_names = [lit.get_name(champion_id) for champion_id in game[
            ['Blue_Champion_1', 'Blue_Champion_2', 'Blue_Champion_3', 'Blue_Champion_4', 'Blue_Champion_5']]]
        red_champions_names = [lit.get_name(champion_id) for champion_id in game[
            ['Red_Champion_1', 'Red_Champion_2', 'Red_Champion_3', 'Red_Champion_4', 'Red_Champion_5']]]

        print(f"{game['Region']}: {blue_champions_names} vs {red_champions_names}")


def main():
    regions = ['EMEA', 'China', 'Korea', 'North America']
    year = datetime.now().year

    most_recent_tournaments = find_most_recent_tournament(regions, year)
    most_recent_games = get_most_recent_games(most_recent_tournaments)
    most_recent_games.to_csv('most_recent_games.csv', index=False)
    print(f"\n {most_recent_games}")
    if testing:
        check_id_matching(most_recent_games)


if __name__ == "__main__":
    testing = False
    main()
