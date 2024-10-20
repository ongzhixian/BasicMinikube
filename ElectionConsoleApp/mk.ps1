# Script to "make" assembly
param(
	[switch]$Debug,
	[string]$versionFilePath = "./version.txt"
)

$projectName = "ElectionConsoleApp"

$main = {

	Push-Location
	Set-Location $PSScriptRoot

	Write-Debug "versionFilePath is: $versionFilePath" 

	$lastVersion = Get-LastVersion $versionFilePath
	$nextVersion = Update-Version $lastVersion

	Write-Host "Building version: [$nextVersion]"
	
	Build-Assembly $nextVersion
	
	#Build-NugetPackage $nextVersion

	#$nugetPackagePath = ".\out\$projectName.$nextVersion.nupkg"
	#PublishTo-NugetOrg $nugetPackagePath
	#PublishTo-LocalFeed $nugetPackagePath
	
	$nextVersion.ToString() | Out-File $versionFilePath

	Pop-Location
}


# SCRIPT HELPER FUNCTIONS

## DOTNET FUNCTIONS

Function Build-Assembly($version) {
	dotnet restore 
	dotnet build -p:AssemblyVersion=$version --nologo --no-restore --configuration Release
}

Function Build-NugetPackage($version) {
	Write-Host "Build-NugetPackage"
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