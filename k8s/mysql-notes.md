# Notes for deploying mysql in kubernetes

You will need:

1.  Stateful
2.  Persistent volume claim
2.  Persistent volume?
2.  Secret
2.  Service

https://hub.docker.com/_/mysql


## Secrets

Powershell script to decode:
`[System.Text.Encoding]::ASCII.GetString([convert]::FromBase64String("YWRtaW4="))`

To encode
`[Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes("Pass1234"))`