# INSECURE -- CONTAINS SECRETS
import signal
import io
import time
import json
import urllib.request

import os

from configuration_loader import load_config
from bucket_storage import upload_to_bucket


# BUSINESS LOGIC FUNCTIONS

def get_instruments(config_json):
    url = f"{config_json['oanda']['endpoint']}/v3/accounts/{config_json['oanda']['account']}/instruments"
    print(url)
    headers = {
        'Content-Type': 'application/json',
        'Authorization': f"Bearer {config_json['oanda']['access_token']}"
    }
    print(headers)

    # data = urllib.parse.urlencode(values)
    # data = data.encode('ascii')
    # req = urllib.request.Request(url='http://localhost:8080', data=DATA, method='PUT')

    req = urllib.request.Request(url=url, headers=headers, method='GET')
    with urllib.request.urlopen(req) as response, open('download-instruments.json', 'wb') as out_file:
        response_data = response.read()
        out_file.write(response_data)
        #print(response_data)
        return response_data


# def upload_to_bucket(config_json, data_stream, data_length):
#     client = Minio(config_json["minio"]["endpoint"],
#         access_key=config_json["minio"]["access_key"],
#         secret_key=config_json["minio"]["secret_key"],
#         secure=False
#     )

#     # The file to upload, change this path if needed
#     source_file = "C:/src/github.com/ongzhixian/elvis/my-test-file.txt"

#     # The destination bucket and filename on the MinIO server
#     bucket_name = "test"
#     destination_file = "instruments.json"
#     client.put_object(bucket_name, destination_file, data=data_stream, length=data_length, content_type="application/json")
#     print(
#         source_file, "successfully uploaded as object",
#         destination_file, "to bucket", bucket_name,
#     )

#
# def write_config():
#     """Not used; """
#     config = {
#         "minio" : {
#             "endpoint": "localhost:19000",
#             "access_key" : "<PLACEHOLDER>",
#             "secret_key" : "<PLACEHOLDER>",
#             "secure": False
#         },
#         "oanda" : {
#             "endpoint": "https://api-fxpractice.oanda.com",
#             "account": "101-003-11976008-005",
#             "access_token": "<PLACEHOLDER>"
#         }
#     }
#     import json 
#     with open('config.json', 'w', encoding='utf-8') as out_file:
#         json.dump(config, out_file, indent=4)


program_terminating = False

def terminate_program(signal_number, _):
    global program_terminating 
    program_terminating = True
    print(f"Shutting down flag raised ({signal_number})")


import socket
def is_port_open(host, port):
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.settimeout(1)  # Set a timeout of 1 second
    try:
        s.connect((host, port))
        return True
    except socket.error:
        return False
    finally:
        s.close()

if __name__ == "__main__":
    signal.signal(signal.SIGINT, terminate_program)
    
    # config_json = load_config()
    # instruments_data = get_instruments(config_json)
    # upload_to_bucket(config_json, io.BytesIO(instruments_data), len(instruments_data))
    i = 0
    while not program_terminating:
        i = i + 1
        print(f"Do something {i} ; Program terminating: {program_terminating}")
        x = is_port_open("localhost", 19000)
        print(x)
        time.sleep(5)
