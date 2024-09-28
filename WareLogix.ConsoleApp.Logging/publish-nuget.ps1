# Script to build and package image into Minikube

param(
	[switch]$Debug,
	[switch]$Incr,
	[string]$versionFilePath = "./version.txt"
)

$projectName = "WareLogix.ConsoleApp.Logging"

if ($Debug) { $DebugPreference = 'Continue' }

# Now to think where/how to control this version number
# OK, let just keep track of it using a text file call `version.txt`
$main = {

	Push-Location
	Set-Location $PSScriptRoot

	Write-Debug "versionFilePath is: $versionFilePath" 

	$lastVersion = Get-LastVersion $versionFilePath
	$nextVersion = Update-Version $lastVersion

	Write-Host "Next version: [$nextVersion]"
	
	Build-Assembly $nextVersion
	Build-NugetPackage $nextVersion

	$nugetPackagePath = ".\out\$projectName.$nextVersion.nupkg"
	PublishTo-NugetOrg $nugetPackagePath
	PublishTo-LocalFeed $nugetPackagePath
	
	$nextVersion.ToString() | Out-File $versionFilePath

	Pop-Location
}


# SCRIPT HELPER FUNCTIONS

## DOTNET FUNCTIONS

Function Build-Assembly($version) {
	dotnet restore 
	dotnet build -p:AssemblyVersion=$version --clp:NoSummary --nologo --no-restore --configuration Release
}

Function Build-NugetPackage($version) {
	dotnet pack -p:PackageVersion=$version --output out --nologo --no-restore --no-build --configuration Release
}

Function PublishTo-NugetOrg($packagePath) {
	$apiKey = $($(dotnet user-secrets list --json ) -replace '//(BEGIN|END)' | ConvertFrom-Json)."nuget-warelogix-api-key"
	dotnet nuget push $packagePath --source https://api.nuget.org/v3/index.json --api-key $apiKey
}

Function PublishTo-LocalFeed($packagePath) {
	nuget add $packagePath -Source D:\nuget-repository
}


## VERSIONING FUNCTIONS

Function Get-LastVersion($versionFilePath) {
	
	if (Test-Path $versionFilePath) {
		$versionText = Get-Content $versionFilePath
		Write-Debug "$versionFilePath = `"$versionText`""

		try	{
			$lastVersion = [Version]::new($versionText)
		}
		catch [System.ArgumentException], [System.FormatException] {
			$lastVersion = [version]::new("0.0")
		}
		catch {
			Write-Host $_.Exception
		}
	}

	Write-Debug "Last version was $lastVersion"
	return $lastVersion
}


Function Update-Version($lastVersion) {
	
	$nextVersion = [version]::new($lastVersion.Major, $lastVersion.Minor, $lastVersion.Build + 1)
	
	Write-Debug "Next version will be $nextVersion"
	return $nextVersion
}


# MAIN

& $main

# END-OF-SCRIPT

# Other ideas:
# Auto-versioning based on what is available in Minikube
# This sounds good until we remember that it might not necessarily be Kubenetes != Minikube
# And also `minikube *` commands are all kind of slow
# $a = $(minikube image ls | Select-String featured-console-app)[0]

# minikube image rm featured-console-app:1