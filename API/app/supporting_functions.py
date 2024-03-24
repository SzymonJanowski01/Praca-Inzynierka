from typing import Optional


def match_champions_with_ids(champions_list: list[str]) -> list[int]:
    champions_names = ["None",  "Annie", "Kayle", "Xerath", "Shyvana", "Ahri", "Graves", "Fizz", "Volibear", "Rengar",
                       "MasterYi", "Varus", "Nautilus", "Viktor", "Sejuani", "Fiora", "Ziggs", "Lulu", "Draven",
                       "Alistar", "Hecarim", "KhaZix", "Darius", "Jayce", "Lissandra", "Ryze", "Diana", "Quinn",
                       "Syndra", "AurelionSol", "Sion", "Kayn", "Zoe", "Zyra", "KaiSa", "Seraphine", "Sivir", "Gnar",
                       "Zac", "Yasuo", "Soraka", "VelKoz", "Taliyah", "Camille", "Akshan", "Teemo", "Tristana",
                       "Warwick", "Olaf", "Nunu", "BelVeth", "Braum", "Jhin", "Kindred", "MissFortune", "Ashe", "Zeri",
                        "Jinx", "TahmKench", "Tryndamere", "Briar", "Viego", "Senna", "Lucian", "Zed", "Jax", "Kled",
                       "Ekko", "Qiyana", "Morgana", "Vi", "Zilean", "Aatrox", "Nami", "Azir", "Singed", "Evelynn",
                       "Twitch", "Galio", "Karthus", "ChoGath", "Amumu", "Rammus", "Anivia", "Shaco", "Yuumi",
                       "DrMundo", "Samira", "Sona", "Kassadin", "Irelia", "TwistedFate", "Janna", "Gangplank", "Thresh",
                       "Corki", "Illaoi", "RekSai", "Ivern", "Kalista", "Karma", "Bard", "Taric", "Veigar", "Trundle",
                       "Rakan", "Xayah", "XinZhao", "Swain", "Caitlyn", "Ornn", "Sylas", "Neeko", "Aphelios", "Rell",
                       "Blitzcrank", "Malphite", "Katarina", "Pyke", "Nocturne", "Maokai", "Renekton", "JarvanIV",
                       "Urgot", "Elise", "Orianna", "Wukong", "Brand", "LeeSin", "Vayne", "Rumble", "Cassiopeia",
                       "LeBlanc", "Vex", "Skarner", "Heimerdinger", "Nasus", "Nidalee", "Udyr", "Yone", "Poppy",
                       "Gragas", "Vladimir", "Pantheon", "Ezreal", "Mordekaiser", "Yorick", "Akali", "Kennen", "Garen",
                       "Sett", "Lillia", "Gwen", "RenataGlasc", "Leona", "Nilah", "KSante", "Fiddlesticks",
                       "Malzahar", "Smolder", "Milio", "Talon", "Hwei", "Riven", "Naafiri", "KogMaw", "Shen", "Lux"]

    champions_ids = [0, 1, 10, 101, 102, 103, 104, 105, 106, 107, 11, 110, 111, 112, 113, 114, 115, 117, 119, 12, 120,
                     121, 122, 126, 127, 13, 131, 133, 134, 136, 14, 141, 142, 143, 145, 147, 15, 150, 154, 157, 16,
                     161, 163, 164, 166, 17, 18, 19, 2, 20, 200, 201, 202, 203, 21, 22, 221, 222, 223, 23, 233, 234,
                     235, 236, 238, 24, 240, 245, 246, 25, 254, 26, 266, 267, 268, 27, 28, 29, 3, 30, 31, 32, 33, 34,
                     35, 350, 36, 360, 37, 38, 39, 4, 40, 41, 412, 42, 420, 421, 427, 429, 43, 432, 44, 45, 48, 497,
                     498, 5, 50, 51, 516, 517, 518, 523, 526, 53, 54, 55, 555, 56, 57, 58, 59, 6, 60, 61, 62, 63, 64,
                     67, 68, 69, 7, 711, 72, 74, 75, 76, 77, 777, 78, 79, 8, 80, 81, 82, 83, 84, 85, 86, 875, 876, 887,
                     888, 89, 895, 897, 9, 90, 901, 902, 91, 910, 92, 950, 96, 98, 99]

    champions_ids: list[int] = [champions_ids[champions_names.index(champion)] for champion in champions_list]

    return champions_ids


def convert_to_optional(schema) -> dict:
    return {i: Optional[j] for i, j in schema.__annotations__.items()}


class Paging:
    MAX_SCENARIO_LIMIT: int = 5
    basic_limit: int = 5

    def __init__(self, skip: int = 0, limit: int = basic_limit):
        self.skip = skip
        self.limit = limit
