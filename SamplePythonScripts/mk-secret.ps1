# Script to create Kubernetes secrets from .NET user-secrets for a given secrets Id

$userSecretsId = '47df7034-c1c1-4b87-8373-89f5e42fc9ec'
$basePath = [System.Environment]::ExpandEnvironmentVariables('%APPDATA%')
$secrets_json_path = [System.IO.Path]::Combine($basePath, 'Microsoft', 'UserSecrets', $userSecretsId, 'secrets.json')

if (-not (Test-Path $secrets_json_path))
{
    Write-Host "$secrets_json_path not found; exiting"
    return
}

Write-Debug "Reading from $secrets_json_path"
$secrets_json_content = Get-Content $secrets_json_path

# {
#     "test2": "test2value",
#     "test": "testvalue",
#     "OandaAccessToken": "",
#     "CloudAmqpUrl": "amqps://",
#     "nuget-warelogix-api-key": ""
#   }

$secrets_json = [System.Text.Json.JsonSerializer]::Deserialize($secrets_json_content, [System.Text.Json.JsonElement])

kubectl create secret generic sample-python -o yaml --dry-run=client --save-config `
    --from-literal=test=$($secrets_json.GetProperty("test2").GetString()) `
    --from-literal=key2=$($secrets_json.GetProperty("test2").GetString())  | kubectl apply -f -

# Generate secrets as a file from string
$secret_file_content = @{
	"oanda" = @{
        "access_token" = $secrets_json.GetProperty("OandaAccessToken").GetString()
    }
    "minio" = @{
        "secret_key" = $secrets_json.GetProperty("MinIoSecretKey").GetString()
    }
} | ConvertTo-Json

kubectl create secret generic sample-python2 -o yaml --dry-run=client --save-config `
    --from-literal="secrets.json"=$($secret_file_content) | kubectl apply -f -


########################################
# The LONG way to create/patch secrets using a if-else
# Code left here for reference

#kubectl delete secret sample-python --ignore-not-found

# 2>&1 --> Redirects stderr to stdout
# Then redirects stdout to $null
# This is to suppress any error messages that will appear if resource is not available
# kubectl get secrets sample-python 2>&1 > $null

# if (-not $?) 
# {
#     Write-Host 'Secret missing; creating secret'
#     kubectl create secret generic sample-python `
#     --from-literal=test=$($secrets_json.GetProperty("test2").GetString()) `
#     --from-literal=key2=$($secrets_json.GetProperty("test2").GetString())  
# }
# else {
#     Write-Host 'Secret exists; patching secret'
#     kubectl create secret generic sample-python -o yaml --dry-run=client --save-config `
#     --from-literal=test=$($secrets_json.GetProperty("test2").GetString()) `
#     --from-literal=key2=$($secrets_json.GetProperty("test2").GetString())  | kubectl apply -f -
# }
