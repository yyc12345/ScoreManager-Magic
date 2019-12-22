import hashlib

while True:
    user=input("User: ")
    password=input("Password: ")
    salt=input("Salt: ")
    pwdHash = hashlib.sha256(password.encode()).hexdigest()
    authHash = hashlib.sha256((pwdHash + salt).encode()).hexdigest()
    print("Password hash: {}\nAuth hash: {}".format(pwdHash, authHash))
    print("============================================")