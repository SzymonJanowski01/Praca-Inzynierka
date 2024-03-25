import random

roles_colors = ["Blue", "Red"]
champions_names = ["None", "Annie", "Kayle", "Xerath", "Shyvana", "Ahri", "Graves", "Fizz", "Volibear", "Rengar",
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


def test_create_empty_phases(client):
    response = client.post(
        "api/user/create-user",
        json={"username": "username_test", "email": "test@example.com", "password": "password_test"}
    )

    user_id = response.json.get('user_id')

    response = client.post(
        f"api/scenario/create-scenario/{user_id}",
        json={"name": "scenario_test"}
    )

    scenario_id = response.json.get('scenario_id')

    response = client.post(
        f"/create-empty-phases/{scenario_id}"
    )
    assert response.status_code == 201

    response = client.get(f"/get-scenario-phases/{scenario_id}")
    assert response.status_code == 200


def test_get_recommendations_valid(client):
    response = client.post(
        "api/user/create-user",
        json={"username": "username_test", "email": "test@example.com", "password": "password_test"}
    )

    user_id = response.json.get('user_id')

    response = client.post(
        f"api/phase/recommendations/{user_id}?{user_id}",
        json={"champions_list": random.choices(champions_names, k=10),
              "target_position": f"{random.choice(roles_colors)}_Champion_{random.randint(1, 5)}"}
    )
    assert response.status_code == 200


def test_update_phase_valid(client):
    response = client.post(
        "api/user/create-user",
        json={"username": "username_test", "email": "test@example.com", "password": "password_test"}
    )

    user_id = response.json.get('user_id')

    response = client.post(
        f"api/scenario/create-scenario/{user_id}",
        json={"name": "scenario_test"}
    )

    scenario_id = response.json.get('scenario_id')

    response = client.post(
        f"/create-empty-phases/{scenario_id}"
    )
    assert response.status_code == 201

    response = client.put(
        f"/update-scenario-phases/{scenario_id}",
        json=[
            {
                "phase_index": 0,
                "attributes": {
                    "main_character": "Olaf",
                    "first_alternative_character": "Sett",
                    "second_alternative_character": "Volibear"
                }
            },
            {
                "phase_index": 1,
                "attributes": {
                    "main_character": "Kindred",
                    "first_alternative_character": "Olaf",
                    "second_alternative_character": "KhaZix"
                }
            },
            {
                "phase_index": 2,
                "attributes": {
                    "main_character": "Qiyana",
                    "first_alternative_character": "Swain",
                    "second_alternative_character": "Viktor"
                }
            },
            {
                "phase_index": 3,
                "attributes": {
                    "main_character": "Sivir",
                    "first_alternative_character": "Zeri",
                    "second_alternative_character": "Ziggs"
                }
            },
            {
                "phase_index": 4,
                "attributes": {
                    "main_character": "Ashe",
                    "first_alternative_character": "Amumu",
                    "second_alternative_character": "Alistar"
                }
            },
            {
                "phase_index": 5,
                "attributes": {
                    "main_character": "Olaf",
                    "first_alternative_character": "Ornn",
                    "second_alternative_character": "Rumble"
                }
            },
            {
                "phase_index": 6,
                "attributes": {
                    "main_character": "Rammus",
                    "first_alternative_character": "RekSai",
                    "second_alternative_character": "Sejuani"
                }
            },
            {
                "phase_index": 7,
                "attributes": {
                    "main_character": "Syndra",
                    "first_alternative_character": "VelKoz",
                    "second_alternative_character": "Vex"
                }
            },
            {
                "phase_index": 8,
                "attributes": {
                    "main_character": "Varus",
                    "first_alternative_character": "KaiSa",
                    "second_alternative_character": "Jhin"
                }
            },
            {
                "phase_index": 9,
                "attributes": {
                    "main_character": "Heimerdinger",
                    "first_alternative_character": "Bard",
                    "second_alternative_character": "Nautilus"
                }
            }
        ]
    )
    assert response.status_code == 200


def test_update_phase_invalid(client):
    response = client.post(
        "api/user/create-user",
        json={"username": "username_test", "email": "test@example.com", "password": "password_test"}
    )

    user_id = response.json.get('user_id')

    response = client.post(
        f"api/scenario/create-scenario/{user_id}",
        json={"name": "scenario_test"}
    )

    scenario_id = response.json.get('scenario_id')

    response = client.post(
        f"/create-empty-phases/{scenario_id}"
    )
    assert response.status_code == 201

    response = client.put(
        f"/update-scenario-phases/{scenario_id}",
        json=[
            {
                "phase_index": 0,
                "attributes": {
                    "main_character": 15,
                    "first_alternative_character": "Sett",
                    "second_alternative_character": "Volibear"
                }
            },
            {
                "phase_index": 21,
                "attributes": {
                    "main_character": "Kindred",
                    "first_alternative_character": "Olaf",
                    "second_alternative_character": "KhaZix"
                }
            },
            {
                "phase_index": 2,
                "attributes": {
                    "main_character": "Qiyana",
                    "first_alternative_character": "Swain",
                    "second_alternative_character": "Viktor"
                }
            },
            {
                "phase_index": 3,
                "attributes": {
                    "main_character": "Sivir",
                    "first_alternative_character": "Zeri",
                    "second_alternative_character": "Ziggs"
                }
            },
            {
                "phase_index": 4,
                "attributes": {
                    "main_character": "Ashe",
                    "first_alternative_character": "Amumu",
                    "second_alternative_character": "Alistar"
                }
            }
        ]
    )
    assert response.status_code == 200
