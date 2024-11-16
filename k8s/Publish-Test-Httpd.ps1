# Script to publish contents of a folder to a pod

param(
    # [Parameter(Mandatory = $true, ParameterSetName = 'Command')]
    # [ValidateSet('remove','fwd-registry','help')]
    # [string]$pod
) 

# Get pod name
$podName = kubectl get pods --template '{{range .items}}{{.metadata.name}}{{"\n"}}{{end}}' -l app=httpd

# Aside:
# Another way more straightforward way to get pod name is:
# kubectl get pod -l app=httpd -o name
# However, this has method has the short-coming of prefixing the pod name with 'pod/' like this:
# pod/httpd-585f95dc9d-ddb58
# Hence the preference for using Go-templates above to get pod name

$main = {
	Push-Location
	Set-Location $PSScriptRoot

    # Main script
    $contentDirectory = "test-httpd"
    $contentDestination = "/usr/local/apache2/htdocs/"
    

    # Copy files in local directory to pod
    $publishSourcePath = ".\$contentDirectory\"
    $publishDestinationPath = "$($podName):$($contentDestination)"
    Write-Host "Publishing [$publishSourcePath] to [$publishDestinationPath]"
    
    kubectl cp $publishSourcePath $publishDestinationPath
    #kubectl cp .\k8s\test-httpd httpd-585f95dc9d-ddb58:/usr/local/apache2/htdocs/


    # Copy the contents out of copied directory and then remove directory
    kubectl exec $podName -it -- bash -c "cp -rf $($contentDestination)$contentDirectory/* $contentDestination && rm -R $contentDestination$contentDirectory/"

    # Aside: Why we do the above step
    # kubectl cp copies directory into destination
    # But what we really want is the contents of the directory to be in destination 
    # (yeah, as of 2024-11-16 -- still cannot do this)
    
	Pop-Location
}

# Run main
& $main