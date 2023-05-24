import hashlib
import os

SALT_LENGTH = 32


def password_hashing(password: str):
    password_data = []

    salt = os.urandom(SALT_LENGTH)
    key = hashlib.pbkdf2_hmac(
        'sha256',
        password.encode('utf-8'),
        salt,
        100000
    )

    password_data.append(key)
    password_data.append(salt)

    return password_data


def password_check(password: str, key: bytes, salt: bytes):
    new_key = hashlib.pbkdf2_hmac(
        'sha256',
        password.encode('utf-8'),
        salt,
        100000
    )

    if new_key == key:
        return True
    else:
        return False
