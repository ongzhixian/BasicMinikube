# Build

Which means the correct way if going about with --no-build flags is to do:
$curVer=[System.Version]::Parse((Get-Content .\WareLogix.WebApi.Logging\version))`
;$newVer=[System.Version]::new($curVer.Major, $curVer.Minor, $curVer.Build+1)`
;$newVer.ToString() | Out-File .\WareLogix.WebApi.Logging\version
$newVer.ToString()

dotnet restore .\WareLogix.WebApi.Logging\
dotnet build   .\WareLogix.WebApi.Logging\ --configuration Release --nologo --no-restore -p:Version=1.0.8
dotnet publish .\WareLogix.WebApi.Logging\ --configuration Release --nologo --no-build
dotnet pack    .\WareLogix.WebApi.Logging\ --configuration Release --nologo --no-build --output nupkgs -p:Version=1.0.8



Working!

ATTEMPT 1

Set-Location C:/src/github.com/ongzhixian/BasicMinikube/WeatherForecastWebApi/bin/Release/net6.0/publish
TODO: Copy Dockerfile
minikube image build . -t weather-forecast-webapi:v1 -f ./Dockerfile


ATTEMPT 2

dotnet publish .\WeatherForecastWebApi\ --configuration Release --output .\publish\WeatherForecastWebApi\
cp .\WeatherForecastWebApi\Dockerfile .\publish\WeatherForecastWebApi\
Set-Location publish
minikube image build . -t weather-forecast-webapi:v1 -f ./Dockerfile


ATTEMPT 3

minikube image build .\publish\WeatherForecastWebApi\ -- -t weather-forecast-webapi:v1 -f ./Dockerfile

## Other

minikube image build ./net6.0/publish -t weather-forecast-webapi:v1 -f ./Dockerfile 


https://backstage.forgerock.com/docs/forgeops/7.5/deploy/deploy-scenario-helm-local.html
https://john-cd.com/cheatsheets/Containers/Minikube_Install_on_Windows/#run-minikube



Troubleshooting

minikube start --alsologtostderr --v=2
minikube --alsologtostderr image build . -t weather-forecast-webapi:v1 -f ./Dockerfile


minikube image build file:///C:/src/github.com/ongzhixian/BasicMinikube/WeatherForecastWebApi/bin/Release/net6.0/publish/. -t weather-forecast-webapi:v1 -f ./WeatherForecastWebApi/Dockerfile


minikube image build -t weather-forecast-webapi:v1 -f ./WeatherForecastWebApi/Dockerfile file:///WeatherForecastWebApi/bin/Release/net6.0/publish/.
