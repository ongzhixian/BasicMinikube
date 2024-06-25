param(
    #  [Parameter(Mandatory = $true, ParameterSetName = 'Command')]
    #  [ValidateSet('fwd-dashboard','fwd-registry','jenkins','apache','nginx','run-all','help')]
    [string]$command
) 


function Print-Help() {
    Write-Output @"
    Help
    ========
    Syntax: .\ako.ps1 -command <command>

    Where <command> is one of:
        1. fwd-dashboard
        2. fwd-registry
        3. jenkins
        3. apache
        4. run-all
        5. help
"@
}

function Start-ForwardDashboard {
    $jobName = "FwdDashboard"
    $runningJobs = Get-Job -Name $jobName -ErrorAction SilentlyContinue | Where-Object { $_.State -eq "Running" }
    if ($runningJobs.length -le 0) {
        Write-Host "Port forward Minikube dashboard; Access: http://localhost:18001/api/v1/namespaces/kubernetes-dashboard/services/http:kubernetes-dashboard:/proxy/#/workloads?namespace=default"
        Start-Job -Name $jobName -ScriptBlock { 
            kubectl proxy --address='0.0.0.0' --accept-hosts='^*$' --port=18001
        }
    }
}

function Start-ForwardRegistry {
    $jobName = "FwdRegistry"
    $existingJobs = Get-Job -Name $jobName -ErrorAction SilentlyContinue
    $runningJobs = $existingJobs | Where-Object { $_.State -eq "Running" }
    if ($runningJobs.length -le 0) {

        Write-Host "Port forward Minikube registry; Access: http://localhost:18002/v2/_catalog"
        Start-Job -Name $jobName -ScriptBlock { 
            kubectl port-forward --namespace kube-system service/registry 18002:80
        }
    }
}

function Start-Jenkins {
    $jobName = "Jenkins"
    $existingJobs = Get-Job -Name $jobName -ErrorAction SilentlyContinue
    $runningJobs = $existingJobs | Where-Object { $_.State -eq "Running" }
    if ($runningJobs.length -le 0) {
        Write-Host "Run Jenkins; Access: http://localhost:15080/"
        Start-Job -Name $jobName -ScriptBlock { 
            Set-Location C:\Apps\Jenkins
            java -jar jenkins.war --httpPort=15080
        }
    }
}

function Start-Apache {
    $jobName = "Apache"
    $existingJobs = Get-Job -Name $jobName -ErrorAction SilentlyContinue
    $runningJobs = $existingJobs | Where-Object { $_.State -eq "Running" }
    if ($runningJobs.length -le 0) {
        Write-Host "Run Apache; Access: http://localhost:15080/"
        Start-Job -Name $jobName -ScriptBlock { 
            Set-Location C:\Apps\Apache24
            .\bin\httpd.exe -f C:/Apps/Apache24/conf/httpd.conf
        }
    }
}

function Start-Nginx {
    $jobName = "Nginx"
    $existingJobs = Get-Job -Name $jobName -ErrorAction SilentlyContinue
    $runningJobs = $existingJobs | Where-Object { $_.State -eq "Running" }
    if ($runningJobs.length -le 0) {
        Write-Host "Run Nginx; Access: http://localhost:15080/"
        Start-Job -Name $jobName -ScriptBlock { 
            Set-Location C:\Apps\nginx
            .\nginx.exe
        }
    }
}

function Test-JobRunning($jobName, $status = "Running") {
    $existingJobs = Get-Job -Name $jobName -ErrorAction SilentlyContinue
    $runningJobs = $existingJobs | Where-Object { $_.State -eq "Running" }
    return $runningJobs.length -ge 1
}

function Start-Tunnel {
    $jobName = "MinikubeTunnel"
    if (Test-JobRunning $jobName) {
        Write-Host 'Skip; Existing running job'
        return
    }
    Start-Job -Name $jobName -ScriptBlock { 
        minikube tunnel --profile minikube
    }
}

function Mount-BasicMinikube {
    $jobName = "MountBasicMinikube"
    if (Test-JobRunning $jobName) {
        Write-Host 'Skip; Existing running job'
        return
    }
    Start-Job -Name $jobName -ScriptBlock { 
        minikube mount C:/data/basic-minikube:/data/basic-minikube
    }
}

function Install-Kourier {
    kubectl apply -f https://github.com/knative/net-kourier/releases/download/knative-v1.14.0/kourier.yaml
    kubectl patch configmap/config-network --namespace knative-serving --type merge --patch '{"data":{"ingress-class":"kourier.ingress.networking.knative.dev"}}'

    Wait-ForKourierPods
}

function Install-KnativeServing {
    kubectl apply -f https://github.com/knative/serving/releases/download/knative-v1.14.1/serving-crds.yaml
    kubectl apply -f https://github.com/knative/serving/releases/download/knative-v1.14.1/serving-core.yaml
    kubectl apply -f https://github.com/knative/serving/releases/download/knative-v1.14.1/serving-hpa.yaml

    Wait-ForPods knative-serving

    # Custom config to enable pvc; See configmap for more details
    kubectl apply -f .\k8s\knative\serving\knative-serving-config-features.configmap.yaml
}

function Install-KnativeEventing {
    kubectl apply -f https://github.com/knative/eventing/releases/download/knative-v1.14.2/eventing-crds.yaml
    kubectl apply -f https://github.com/knative/eventing/releases/download/knative-v1.14.2/eventing-core.yaml
    kubectl apply -f https://github.com/knative/eventing/releases/download/knative-v1.14.2/in-memory-channel.yaml
    kubectl apply -f https://github.com/knative/eventing/releases/download/knative-v1.14.2/mt-channel-broker.yaml

    Wait-ForPods knative-eventing
}

function Install-MagicDns {
    kubectl apply -f https://github.com/knative/serving/releases/download/knative-v1.14.1/serving-default-domain.yaml
}

function Show-InstallStatus {
    kubectl get pods -n knative-serving
    kubectl get pods -n knative-eventing
    kubectl get service kourier --namespace kourier-system 
}

function Install-Addons {
    minikube addons enable metrics-server
    minikube addons enable dashboard
    # minikube addons enable ingress
    # minikube addons enable ingress-dns
}

function Update-NrptRule {
    Get-DnsClientNrptRule | Where-Object { $_.Namespace -eq ".internal" } | Foreach-Object { Remove-DnsClientNrptRule -Name $_.Name -Force }
    Add-DnsClientNrptRule -Namespace ".internal" -NameServers "$(minikube ip)"
}



function Wait-ForPods ($targetNamespace) {
    $pods = kubectl get pods -n $targetNamespace -o json | ConvertFrom-Json | Select-Object -ExpandProperty items 
    $nonRunningPods = $pods| Where-Object { $_.status.phase -ne "Running" }

    while ($nonRunningPods.length -gt 0) {
        kubectl get pods -n $targetNamespace
        Write-Output "Waiting for all pods to be running"
        Start-Sleep -Seconds 10
        $pods = kubectl get pods -n $targetNamespace -o json | ConvertFrom-Json | Select-Object -ExpandProperty items 
        $nonRunningPods = $pods| Where-Object { $_.status.phase -ne "Running" }
    }
}


function Wait-ForKourierPods () {
    $pods = kubectl get pods --all-namespaces -l 'app in(3scale-kourier-gateway,3scale-kourier-control)' -o json | ConvertFrom-Json | Select-Object -ExpandProperty items 
    $nonRunningPods = $pods| Where-Object { $_.status.phase -ne "Running" }

    while ($nonRunningPods.length -gt 0) {
        kubectl get pods --all-namespaces -l 'app in(3scale-kourier-gateway,3scale-kourier-control)'
        Write-Output "Waiting for all pods to be running"
        Start-Sleep -Seconds 10
        $pods = kubectl get pods --all-namespaces -l 'app in(3scale-kourier-gateway,3scale-kourier-control)' -o json | ConvertFrom-Json | Select-Object -ExpandProperty items 
        $nonRunningPods = $pods| Where-Object { $_.status.phase -ne "Running" }
    }
}


function Install-WeatherForecastWebApi {
    dotnet publish .\WeatherForecastWebApi\ --configuration Release
    Copy-Item .\WeatherForecastWebApi\Dockerfile .\WeatherForecastWebApi\bin\Release\net6.0\publish\
    Push-Location
    Set-Location C:/src/github.com/ongzhixian/BasicMinikube/WeatherForecastWebApi/bin/Release/net6.0/publish
    minikube image build . -t dev.local/weather-forecast-webapi:v1 -f ./Dockerfile
    Pop-Location

    # kn service update weather-forecast-webapi --port 80

    kubectl apply -f .\k8s\weather-forecast-webapi.configmap.yaml
    kubectl apply -f .\k8s\weather-forecast-webapi.secret.yaml
    kubectl apply -f .\k8s\weather-forecast-webapi.storage.yaml
    
    #kn service create hello --image ghcr.io/knative/helloworld-go:latest --port 8080 --env TARGET=World
    #kn service create weather-forecast-webapi --image dev.local/weather-forecast-webapi:v1 --port=80
    kubectl apply -f .\k8s\weather-forecast-webapi.serving.yaml
}

function Install-Applications {
    Write-Host "TODO: Install applications"

    # Redis / Or Garnet (https://github.com/microsoft/garnet)
    # Kafka
}

function Test-AdminPrivileges {
    if ($env:OS -eq "Windows_NT") {
        $currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
        $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
    }
    return $false
}

function Remove-MinikubeJob ($jobName) {
    $job = Get-Job -Name $jobName
    if ($? -and $job.State -eq "Running") {
        Stop-Job $job.Id
        Remove-Job $job.Id
    }

}

function Remove-DevMinikube {
    Remove-MinikubeJob "MinikubeTunnel"
    Remove-MinikubeJob "MountBasicMinikube"
    minikube delete -p minikube
    Get-Job
}

function Initialize-DevMinikube {
    minikube start --cpus='3' --memory='4096MB' --disk-size='20480mb'
    minikube addons enable registry

    if ($?) { Start-Tunnel }
    if ($?) { Mount-BasicMinikube }
    if ($?) { Install-KnativeServing }
    if ($?) { Install-Kourier }
    if ($?) { Install-MagicDns }
    if ($?) { Install-KnativeEventing }
    if ($?) { Install-Addons }
    if ($?) { Update-NrptRule }
    if ($?) { Install-Applications }
}

########################################
# START OF MAIN SCRIPT

# if (Test-AdminPrivileges -eq $false) {
#     Write-Error "[FAIL]"
#     Write-Output @"
# This script is designed to in Windows PowerShell session with administrative privileges.

# You are seeing this message because you are either:

# a.  Running this script in a non-Windows NT machine
# --OR--
# b.  Running this script without administrative privileges

# Please correct the either conditions before running this script again
# "@
#     return
# }

switch ($command)
{
    "remove" {
        Remove-DevMinikube
    }

    "init" {
        Initialize-DevMinikube
    }

    default {
        Write-Host "remove or init"
    }
}

