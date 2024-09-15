param(
    #  [Parameter(Mandatory = $true, ParameterSetName = 'Command')]
    #  [ValidateSet('fwd-dashboard','fwd-registry','jenkins','apache','nginx','run-all','help')]
    [string]$command
) 

function Install-DotnetEfCli() {
	dotnet tool install --global dotnet-ef

}

function Update-DotnetEfCli() {
	dotnet tool update --global dotnet-ef
	# dotnet add package Microsoft.EntityFrameworkCore.Design
}

switch ($command)
{
    "install" {
        Install-DotnetEfCli
    }
	
	"update" {
        Update-DotnetEfCli
    }

    "init" {
        #Initialize-DevMinikube
    }

    default {
        Write-Host "install or update"
    }
}

