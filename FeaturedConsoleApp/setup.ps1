# Script to build and package image into Minikube

param(
	[switch]$Debug,
	[string]$versionFilePath = "./version.txt"
)

if ($Debug) { $DebugPreference = 'Continue' }

# Now to think where/how to control this version number
# OK, let just keep track of it using a text file call `version.txt`
$main = {
	Write-Debug "versionFilePath is: $versionFilePath" 

	$lastVersion = 	Get-LastVersion $versionFilePath
	$nextVersion = Update-Version $lastVersion

	minikube image build . -t featured-console-app:$nextVersion -f .\Dockerfile
	kubectl set image deployment/featured-console-app featured-console-app=featured-console-app:$nextVersion

	$nextVersion.ToString() | Out-File $versionFilePath
}


# SCRIPT HELPER FUNCTIONS

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
	
	$nextVersion = [version]::new($lastVersion.Major, $lastVersion.Minor + 1)
	
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