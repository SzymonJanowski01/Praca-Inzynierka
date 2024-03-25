import app.supporting_functions as sf


def test_match_champions_with_ids():
    champions_names = ["Ahri", "Kled", "Irelia", "Taric", "Lilia", "KogMaw"]
    expected_ids = [103, 240, 39, 44, 876, 96]

    ids = sf.match_champions_with_ids(champions_names)
    assert ids == expected_ids


def test_match_champions_with_ids_empty():
    champions_names = []

    ids = sf.match_champions_with_ids(champions_names)
    assert ids == []


def test_match_champions_with_names():
    champions_ids = {
        "eu": [103, 240, 39, 44, 876, 96],
        "na": [90, 8, 777, 68, 51, 96],
    }
    expected_names = {
        "eu": ["Ahri", "Kled", "Irelia", "Taric", "Lilia", "KogMaw"],
        "na": ["Malzahar", "Vladimir", "Yone", "Rumble", "Caitlyn"],
    }

    names = sf.match_champions_with_names(champions_ids)
    assert names == expected_names
