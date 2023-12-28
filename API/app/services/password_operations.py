import hashlib
import os

from typing import Dict

SALT_LENGTH = 32


def password_hashing(password: str) -> Dict[str, bytes]:
    """
    Function for hashing provided password using pbkf2_hmac.

    :param password: Provided by the user to undergo encryption.
    :return: Dictionary containing hashed password, and salt in the following order: {"hashed_password": password, "salt": salt}.
    """
    password_data: Dict[str, bytes] = {}

    salt = os.urandom(SALT_LENGTH)
    key = hashlib.pbkdf2_hmac(
        'sha256',
        password.encode('utf-8'),
        salt,
        100000
    )

    password_data['hashed_password'] = key
    password_data['salt'] = salt

    return password_data


def password_check(password: str, key: bytes, salt: bytes) -> bool:
    """
    Function for checking whether one password matches the second one. The function uses correct salt to encrypt newly
    provided password. If the password encrypted this way matches the provided encrypted password, then both passwords
    are the same, as using the same randomly generated salt on the same password while using pbkdf2_hmac will always
    produce the same encrypted password.

    :param password: Password to check if it matches.
    :param key: Correct password for comparison.
    :param salt: Correct salt to encrypt the password being checked.
    :return: True if both password matches, False otherwise.
    """
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
