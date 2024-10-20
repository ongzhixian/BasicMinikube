param(
    [Parameter(mandatory=$true)]
    [string]
    $imageName,
    [string]
    $exclude = 'latest'
)


if ([string]::IsNullOrEmpty($imageName))
{
    Write-Host "Empty image image; Specify a complete or partial image name"
    return
}

Write-Host "Cleaning out image [$imageName]"
$imageList = minikube image ls --format json | ConvertFrom-Json
$filteredImageList = $imageList | Where-Object { $_.repoTags | Where-Object { $_.Contains($imageName) } }
$filteredImageList = $($filteredImageList | Where-Object { -not $( $_.repoTags | Where-Object { $_.Contains($exclude) } ) })

# Remove all images except the one tag with latest
if ($filteredImageList.Length -gt 0)
{
    $filteredImageList | ForEach-Object { 

        $_.repoTags | ForEach-Object {
            minikube image rm $_
            Write-Host "Removed" $_
        }
    }
}
