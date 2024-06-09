# dotnet apps

dotnet new razor -n Recep
dotnet new web -n ReverseProxy
dotnet new webapi -n WeatherForecastWebApi --use-controllers


dotnet new razorclasslib -n WareLogix.RazorLib --support-pages-and-views

## Building container images

minikube

```
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /opt/app

# Copy everything
COPY . ./

ENTRYPOINT ["dotnet", "WeatherForecastWebApi.dll"]
```