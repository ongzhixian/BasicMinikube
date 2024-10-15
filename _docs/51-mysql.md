# MySql

## Setup server

First time run
```
.\bin\mysqld.exe --console --log_syslog=0 --basedir=C:/Apps/mysql --datadir=D:/data/mysql --initialize 
```

Running this command will generate an output like the below.
Note the last line where a temporary password is generated for the 'root' user.
You will need this password to login and change the password.

```
2024-10-11T12:32:54.525007Z 0 [Warning] TIMESTAMP with implicit DEFAULT value is deprecated. Please use --explicit_defaults_for_timestamp server option (see documentation for more details).
2024-10-11T12:32:54.624993Z 0 [Warning] InnoDB: New log files created, LSN=45790
2024-10-11T12:32:54.652767Z 0 [Warning] InnoDB: Creating foreign key constraint system tables.
2024-10-11T12:32:54.729682Z 0 [Warning] No existing UUID has been found, so we assume that this is the first time that this server has been started. Generating a new UUID: f05a0a9b-87cc-11ef-ab3d-84a9383abf8e.
2024-10-11T12:32:54.732630Z 0 [Warning] Gtid table is not ready to be used. Table 'mysql.gtid_executed' cannot be opened.
2024-10-11T12:32:55.460186Z 0 [Warning] A deprecated TLS version TLSv1 is enabled. Please use TLSv1.2 or higher.
2024-10-11T12:32:55.460353Z 0 [Warning] A deprecated TLS version TLSv1.1 is enabled. Please use TLSv1.2 or higher.
2024-10-11T12:32:55.463492Z 0 [Warning] CA certificate ca.pem is self signed.
2024-10-11T12:32:55.598267Z 1 [Note] A temporary password is generated for root@localhost: =laUojkSd4+o
```


## Run server

Subsequent run
```
.\bin\mysqld.exe --console --log_syslog=0 --basedir=C:/Apps/mysql --datadir=D:/data/mysql
```

## Shutdown server

```
.\bin\mysqladmin.exe -u root shutdown --password="<password>"
```


## Initial login and change password

```ps1; Logging in
.\bin\mysql.exe -u root --password="=laUojkSd4+o"
```

```sql ;changing password
ALTER USER 'root'@'localhost' IDENTIFIED BY '<new-password>';
```

## Creating another user account

```sql
CREATE USER 'developer'@'%' IDENTIFIED BY '<account-password>';
GRANT ALL PRIVILEGES ON *.* TO 'developer'@'%' WITH GRANT OPTION;
FLUSH PRIVILEGES;
```

