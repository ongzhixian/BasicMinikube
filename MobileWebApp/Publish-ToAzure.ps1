$main = {
	Push-Location
	Set-Location $PSScriptRoot

    # Main script

    Write-Host [PUBLISH PROJECT]  -ForegroundColor DarkYellow
    if (Test-Path .\bin\Release\net9.0\publish\)
    {
        Remove-Item -Recurse -Force .\bin\Release\net9.0\publish\
    }
    dotnet publish --configuration Release
    
    
    Write-Host [PUBLISH AZURE]  -ForegroundColor DarkYellow
    Set-Location .\bin\Release\net9.0\publish\
    az webapp up --resource-group telera-resource-group --plan telera-app-service-plan --name telera-mobile-app --runtime "dotnet:9"

	Pop-Location
}

# Run main
& $main
