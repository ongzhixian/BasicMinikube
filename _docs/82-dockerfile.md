# Mutlt-stage Dockerfile

Example for Dockerfile and .dockerignore.
To build container file you NEED Dockerfile.
But if you do not want to have the Dockerfile present in the result image, 
we have to do a multi-stage build.


```Dockerfile
#
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /opt/src
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o /opt/publish


# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /opt/app
COPY --from=build-env /opt/publish .
ENTRYPOINT ["dotnet", "ConsoleJob.dll"]
```

```.dockerignore
# .dockerignore
bin/
obj/
```