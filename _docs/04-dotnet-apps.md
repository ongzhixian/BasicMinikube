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


## Deployment

Set-Location C:/src/github.com/ongzhixian/BasicMinikube/WeatherForecastWebApi/bin/Release/net6.0/publish
TODO: Copy Dockerfile from src
minikube image build . -t weather-forecast-webapi:v1 -f ./Dockerfile

kubectl create deployment weather-forecast-webapi --image=docker.io/library/weather-forecast-webapi:v1
kubectl expose deployment weather-forecast-webapi --type=NodePort --port=80
kubectl delete ingress weather-forecast-webapi
kubectl create ingress weather-forecast-webapi --rule="weather-forecast-webapi.lan/*=weather-forecast-webapi:80"

http://172.23.157.79:31343/WeatherForecast
http://weather-forecast-webapi.lan/WeatherForecast


kubectl create deployment web --image=gcr.io/google-samples/hello-app:1.0
kubectl get deployment web 
kubectl expose deployment web --type=NodePort --port=8080
kubectl create ingress example-ingress --rule="hello-world.lan/*=web:8080"


Add-DnsClientNrptRule -Namespace ".lan" -NameServers "$(minikube ip)"