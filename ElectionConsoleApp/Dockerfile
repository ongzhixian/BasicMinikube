FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /opt/src
COPY . ./
RUN dotnet restore
RUN dotnet publish ElectionConsoleApp.csproj --nologo --no-restore --configuration Release --output /opt/publish 

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime-env
WORKDIR /opt/app
COPY --from=build-env /opt/publish .
ENTRYPOINT ["dotnet", "ElectionConsoleApp.dll"]