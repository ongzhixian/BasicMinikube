param(
    [string]$command
) 


switch ($command)
{
    "webapi" {
        # Measure-Command {
        #     dotnet publish .\WeatherForecastWebApi\ --configuration Release
        #     pushd .
        #     cd .\WeatherForecastWebApi\bin\Release\net6.0\publish\
        #     minikube image build . -t weather-forecast-webapi:v1 -f ./Dockerfile
        #     popd
        #     kubectl rollout restart deployment weather-forecast-webapi
        # }
        dotnet publish .\WeatherForecastWebApi\ --configuration Release
        pushd .
        cd .\WeatherForecastWebApi\bin\Release\net6.0\publish\
        minikube image build . -t weather-forecast-webapi:v1 -f ./Dockerfile
        popd
        kubectl rollout restart deployment weather-forecast-webapi
    }
    

    default {
        Write-Host "Do nothing"
    }
}