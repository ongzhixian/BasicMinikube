FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /opt/src
COPY . ./
RUN dotnet restore
RUN dotnet publish BackgroundServiceWebApi.csproj --nologo --no-restore --configuration Release --output /opt/publish 

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime-env
WORKDIR /opt/app
COPY --from=build-env /opt/publish .
ENTRYPOINT ["dotnet", "BackgroundServiceWebApi.dll"]