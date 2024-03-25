
def test_create_scenario_valid(client):
    response = client.post(
        "api/user/create-user",
        json={"username": "username_test", "email": "test@example.com", "password": "password_test"}
    )

    user_id = response.json.get('user_id')

    response = client.get(f"api/scenario/get-get-user-scenarios/{user_id}")
    assert response.status_code == 404

    response = client.post(
        f"api/scenario/create-scenario/{user_id}",
        json={"name": "scenario_test"}
    )
    assert response.status_code == 201

    response = client.get(f"api/scenario/get-get-user-scenarios/{user_id}")
    assert response.status_code == 200
    assert len(response.json()) == 1


def test_get_user_scenarios_names(client):
    response = client.post(
        "api/user/create-user",
        json={"username": "username_test", "email": "test@example.com", "password": "password_test"}
    )

    user_id = response.json.get('user_id')

    response = client.get(f"api/scenario/get-get-user-scenarios/{user_id}")
    assert response.status_code == 404

    response = client.post(
        f"api/scenario/create-scenario/{user_id}",
        json={"name": "scenario_test"}
    )
    assert response.status_code == 201

    response = client.post(
        f"api/scenario/create-scenario/{user_id}",
        json={"name": "scenario_test1"}
    )
    assert response.status_code == 201

    response = client.post(
        f"api/scenario/create-scenario/{user_id}",
        json={"name": "scenario_test2"}
    )
    assert response.status_code == 201

    response = client.get("api/scenario/get-user-scenarios-names/user_id")
    assert response.status_code == 200
    assert len(response.json()) == 3


def test_update_scenario(client):
    response = client.post(
        "api/user/create-user",
        json={"username": "username_test", "email": "test@example.com", "password": "password_test"}
    )

    user_id = response.json.get('user_id')

    response = client.post(
        f"api/scenario/create-scenario/{user_id}",
        json={"name": "scenario_test"}
    )
    assert response.status_code == 201

    scenario_id = response.json.get('scenario_id')

    response = client.put(
        f"api/scenario/update-scenario/{scenario_id}",
        json={"name": "scenario_test_updated"}
    )
    assert response.status_code == 200


def test_update_scenario_invalid(client):
    response = client.put(
        f"api/scenario/update-scenario/invalid_id",
        json={"name": "scenario_test_updated"}
    )
    assert response.status_code == 404


def test_delete_scenario_valid(client):
    response = client.post(
        "api/user/create-user",
        json={"username": "username_test", "email": "test@example.com", "password": "password_test"}
    )

    user_id = response.json.get('user_id')

    response = client.post(
        f"api/scenario/create-scenario/{user_id}",
        json={"name": "scenario_test"}
    )
    assert response.status_code == 201

    scenario_id = response.json.get('scenario_id')

    response = client.get(f"api/scenario/get-get-user-scenarios/{user_id}")
    assert response.status_code == 200
    assert len(response.json()) == 1

    response = client.delete(f"api/scenario/delete-scenario/{scenario_id}")
    assert response.status_code == 200
    assert len(response.json()) == 0


def test_delete_scenario_invalid(client):
    response = client.delete(f"api/scenario/delete-scenario/invalid_id")
    assert response.status_code == 404
