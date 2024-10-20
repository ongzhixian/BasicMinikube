import os
import json

# FUNCTIONS TO GET SECRETS 

def get_secrets_json_from_files():
    print("TODO: Read secrets from volumes mounted")
    secrets_json_file_path = './.secrets.json'
    if not os.path.exists(secrets_json_file_path):
        raise Exception(f"File '{secrets_json_file_path}' missing") 
    with open(secrets_json_file_path, 'r', encoding='utf8') as in_file:
        secrets_json = json.load(in_file)
        print(f"secrets_json -- {secrets_json}")
        return secrets_json

def get_secrets_json_from_user_secrets():
    userSecretsId = '47df7034-c1c1-4b87-8373-89f5e42fc9ec'
    basePath = os.path.expandvars('$APPDATA')
    secrets_json_path = os.path.join(basePath, 'Microsoft', 'UserSecrets', userSecretsId, 'secrets.json')
    with open(secrets_json_path, 'r', encoding='utf-8-sig') as in_file:
        json_data = json.loads(in_file.read())
        secrets_json = {}
        if 'OandaAccessToken' in json_data:
            secrets_json['oanda'] = {}
            secrets_json['oanda']['access_token'] = json_data['OandaAccessToken']
        if 'MinIoSecretKey' in json_data:
            secrets_json['minio'] = {}
            secrets_json['minio']['secret_key'] = json_data['MinIoSecretKey']
        print("Read secrets from user-secrets")
        return secrets_json


def load_config():
    json_data = {}
    with open('./config.json', 'r', encoding='utf8') as in_file:
        json_data = json_data | json.load(in_file)
    if "KUBERNETES_SERVICE_HOST" in os.environ:
        secrets_json = get_secrets_json_from_files()
    else:
        secrets_json = get_secrets_json_from_user_secrets()
    
    # Merge secrets into config.json
    # Merging of dictionaries in Python is shallow; A deep merge is kind of complex
    # So we cheat and just merge the key sections individually
    json_data['oanda'] = secrets_json['oanda'] | json_data['oanda']
    json_data['minio'] = secrets_json['minio'] | json_data['minio']
    print(f"merge_data   -- {json_data}")
    return json_data

