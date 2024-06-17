# MinIO



## Launch Server
Use this command to start a local MinIO instance in the C:\minio folder:

minio.exe server C:\data\minio --console-address :9001

Note: This is a blocking execution

Output
```
API: http://192.168.79.11:9000  http://172.20.32.1:9000  http://127.0.0.1:9000
   RootUser: minioadmin
   RootPass: minioadmin

WebUI: http://192.168.79.11:9001 http://172.20.32.1:9001 http://127.0.0.1:9001
   RootUser: minioadmin
   RootPass: minioadmin
```

## Client

Use mc.exe alias set to quickly authenticate and connect to the MinIO deployment.

mc.exe alias set local http://127.0.0.1:9000 minioadmin minioadmin
mc.exe admin info local

The mc.exe alias set takes four arguments:

    The name of the alias

    The hostname or IP address and port of the MinIO server

    The Access Key for a MinIO user

    The Secret Key for a MinIO user


# Reference

MinIO Object Storage for Windows
https://min.io/docs/minio/windows/index.html

Exe
Server
https://dl.min.io/server/minio/release/windows-amd64/minio.exe
Client
https://dl.min.io/client/mc/release/windows-amd64/mc.exe