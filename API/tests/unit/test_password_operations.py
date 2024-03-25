import app.services.password_operations as po


def test_hash_password():
    password = "password_test"
    hashed_password = po.password_hashing(password)
    assert hashed_password.get("hashed_password") is not None
    assert hashed_password.get("salt") is not None
    assert hashed_password.get("hashed_password") != password
    assert hashed_password.get("salt") != password


def test_password_check_valid():
    password = "password_test"
    hashed_password = po.password_hashing(password)
    assert po.password_check(password, hashed_password.get("hashed_password"), hashed_password.get("salt")) is True


def test_password_check_invalid():
    password = "password_test"
    hashed_password = po.password_hashing(password)
    assert po.password_check("invalid_password", hashed_password.get("hashed_password"),
                             hashed_password.get("salt")) is False
