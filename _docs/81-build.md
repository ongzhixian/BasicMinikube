Which means the correct way if going about with --no-build flags is to do:
$curVer=[System.Version]::Parse((Get-Content .\WareLogix.WebApi.Logging\version))`
;$newVer=[System.Version]::new($curVer.Major, $curVer.Minor, $curVer.Build+1)`
;$newVer.ToString() | Out-File .\WareLogix.WebApi.Logging\version
$newVer.ToString()

dotnet restore .\WareLogix.WebApi.Logging\
dotnet build   .\WareLogix.WebApi.Logging\ --configuration Release --nologo --no-restore -p:Version=1.0.8
dotnet publish .\WareLogix.WebApi.Logging\ --configuration Release --nologo --no-build
dotnet pack    .\WareLogix.WebApi.Logging\ --configuration Release --nologo --no-build --output nupkgs -p:Version=1.0.8
