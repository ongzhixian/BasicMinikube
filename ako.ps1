param(
     [Parameter(Mandatory = $true, ParameterSetName = 'Command')]
     [ValidateSet('fwd-dashboard','fwd-registry','jenkins','apache','nginx','run-all','help')]
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


switch ($command)
{
    "fwd-dashboard" {
        Start-ForwardDashboard
    }
    
    "fwd-registry" {
        Start-ForwardRegistry
    }

    "jenkins" {
        Start-Jenkins
    }

    "apache" {
        Start-Apache
    }

    "nginx" {
        Start-Nginx
    }

    "run-all" {
        Start-ForwardDashboard
        Start-ForwardRegistry
        Start-Jenkins
    }

    default {
        Print-Help
    }
}