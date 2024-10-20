from minio import Minio
from minio.error import S3Error

def upload_to_bucket(config_json, data_input_stream, data_input_length):
    client = Minio(config_json["minio"]["endpoint"],
        access_key=config_json["minio"]["access_key"],
        secret_key=config_json["minio"]["secret_key"],
        secure=False
    )

    # The file to upload, change this path if needed
    # source_file = "C:/src/github.com/ongzhixian/elvis/my-test-file.txt"

    # The destination bucket and filename on the MinIO server
    bucket_name = "test"
    destination_file = "instruments.json"
    client.put_object(bucket_name, destination_file, data=data_input_stream, length=data_input_length, content_type="application/json")
    print("Data successfully uploaded as object", destination_file, "to bucket", bucket_name,)
