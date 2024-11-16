param(
    # [Parameter(Mandatory = $true, ParameterSetName = 'Command')]
    # [ValidateSet('remove','fwd-registry','help')]
    # [string]$pod
) 


# kubectl get pods --template '{{range .items}}{{.metadata.name}}{{"\n"}}{{end}}'
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
    kubectl exec $podName -it -- bash

	Pop-Location
}

# Run main
& $main