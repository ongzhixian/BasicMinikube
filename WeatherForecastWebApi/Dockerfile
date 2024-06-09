# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /opt/app

# Copy everything
COPY . ./

ENTRYPOINT ["dotnet", "WeatherForecastWebApi.dll"]