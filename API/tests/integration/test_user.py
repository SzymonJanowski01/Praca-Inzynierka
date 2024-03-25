
def test_create_user_valid(client):
    response = client.get("api/user/get-users")
    assert response.status_code == 200
    assert response.json() == []

    response = client.post(
        "api/user/create-user",
        json={"username": "username_test", "email": "test@example.com", "password": "password_test"}
    )
    assert response.status_code == 201

    response = client.get(f"api/user/get-user/{response.json.get('user_id')}")
    assert response.status_code == 200

    response = client.get("/api/user/get-users")
    assert response.status_code == 200
    assert len(response.json()) == 1


def test_create_user_invalid(client):
    response = client.post(
        "api/user/create-user",
        json={"username": "username_test", "email": "test@example.com", "password": "password_test"}
    )
    assert response.status_code == 201

    response = client.post(
        "api/user/create-user",
        json={"username": "username_test", "email": "test@example.com", "password": "password_test"}
    )
    assert response.status_code == 409


def test_credentials_check_valid(client):
    response = client.post(
        "api/user/create-user",
        json={"username_or_email": "username_test", "password": "password_test"}
    )
    assert response.status_code == 201

    response = client.post(
        "api/user/check-credentials",
        json={"username_or_email": "username_test", "password": "password_test"}
    )
    assert response.status_code == 200


def test_credentials_check_invalid(client):
    response = client.post(
        "api/user/create-user",
        json={"username_or_email": "username_test", "password": "password_test"}
    )
    assert response.status_code == 201

    response = client.post(
        "api/user/check-credentials",
        json={"username_or_email": "username_test", "password": "password_test_invalid"}
    )
    assert response.status_code == 401


def test_credentials_check_not_found(client):
    response = client.post(
        "api/user/check-credentials",
        json={"username_or_email": "username_test", "password": "password_test"}
    )
    assert response.status_code == 404


def test_update_user_valid(client):
    response = client.post(
        "api/user/create-user",
        json={"username": "username_test", "email": "test@example.com", "password": "password_test"}
    )
    assert response.status_code == 201

    response = client.put(
        f"api/user/update-user/{response.json.get('user_id')}",
        json={"username": "username_test_updated"}
    )
    assert response.status_code == 200


def test_update_user_invalid(client):
    response = client.put(
        f"api/user/update-user/invalid_id",
        json={"username": "username_test_updated"}
    )
    assert response.status_code == 404


def test_update_user_invalid_conflict(client):
    response = client.post(
        "api/user/create-user",
        json={"username": "username_test", "email": "test@example.com", "password": "password_test"}
    )
    assert response.status_code == 201

    response = client.post(
        "api/user/create-user",
        json={"username": "username_test2", "email": "test@example2.com", "password": "password_test"}
    )
    assert response.status_code == 201

    response = client.put(
        f"api/user/update-user/{response.json.get('user_id')}",
        json={"email": "test@example.com"}
    )
    assert response.status_code == 409


def test_update_user_invalid_data(client):
    response = client.post(
        "api/user/create-user",
        json={"username": "username_test", "email": "test@example.com", "password": "password_test"}
    )
    assert response.status_code == 201

    response = client.put(
        f"api/user/update-user/{response.json.get('user_id')}",
        json={"email": 15}
    )
    assert response.status_code == 422


def test_get_user_invalid(client):
    response = client.get(f"api/user/get-user/invalid_id")
    assert response.status_code == 404


def test_delete_user_valid(client):
    response = client.post(
        "api/user/create-user",
        json={"username": "username_test", "email": "test@example.com", "password": "password_test"}
    )
    assert response.status_code == 201

    response = client.delete(f"api/user/delete-user/{response.json.get('user_id')}")
    assert response.status_code == 200


def test_delete_user_invalid(client):
    response = client.delete(f"api/user/delete-user/invalid_id")
    assert response.status_code == 404
