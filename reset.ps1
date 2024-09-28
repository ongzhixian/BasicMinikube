# # Script when resetting Minikube
param(
    #[Parameter(Mandatory = $true, ParameterSetName = 'Command')]
    [ValidateSet('remove','fwd-registry','help')]
    [string]$command
) 

function Remove-DevMinikube {
    minikube stop ; minikube delete
}
function Initialize-DevMinikube {
    minikube start
    minikube addons enable registry
    minikube addons enable metrics-server
    minikube addons enable dashboard

    
    minikube image load D:\container-images\dotnet-sdk-8.0.tar
    minikube image load D:\container-images\dotnet-runtime-8.0.tar
    minikube image load D:\container-images\busybox-1.36.tar
    minikube image load D:\container-images\redis-7.4.0.tar

    
    kubectl create deployment redis --image=redis:7.4.0

    # Default cluster
    # Creating hyperv VM (CPUs=2, Memory=6000MB, Disk=20000MB)

    # minikube start --cpus='3' --memory='4096MB' --disk-size='20480mb'
    # minikube addons enable registry

    # if ($?) { Start-Tunnel }
    # if ($?) { Mount-BasicMinikube }
    # if ($?) { Install-KnativeServing }
    # if ($?) { Install-Kourier }
    # if ($?) { Install-MagicDns }
    # if ($?) { Install-KnativeEventing }
    # if ($?) { Install-Addons }
    # if ($?) { Update-NrptRule }
    # if ($?) { Install-Applications }

    #minikube image save library/ubuntu:24.04 D:\container-images\ubuntu-24.04.tar
}


# ########################################
# # START OF MAIN SCRIPT

# # if (Test-AdminPrivileges -eq $false) {
# #     Write-Error "[FAIL]"
# #     Write-Output @"
# # This script is designed to in Windows PowerShell session with administrative privileges.

# # You are seeing this message because you are either:

# # a.  Running this script in a non-Windows NT machine
# # --OR--
# # b.  Running this script without administrative privileges

# # Please correct the either conditions before running this script again
# # "@
# #     return
# # }

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

