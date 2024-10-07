dotnet sonarscanner begin /k:"RedisConsoleApp" /d:sonar.host.url="http://127.0.0.1:9000"  /d:sonar.token="sqp_0f212a9cbad26a73b8743e04a17f8c93d1b47509"
dotnet build .\RedisConsoleApp\
dotnet sonarscanner end /d:sonar.token="sqp_0f212a9cbad26a73b8743e04a17f8c93d1b47509"
