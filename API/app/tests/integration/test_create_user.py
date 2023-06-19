
def test_create_user(client):
    response = client.get("api/user/get-users")
    assert response.status_code == 200
    assert response.json() == []

    response = client.post(
        "api/user/create-user",
        json={"username": "username_test", "email": "test@example.com", "password": "password_test"}
    )
    assert response.status_code == 200

    response = client.get(f"api/user/get-user/{response.json.get('user_id')}")
    assert response.status_code == 200

    response = client.get("/api/user/get-users")
    assert response.status_code == 200
    assert len(response.json()) == 1
