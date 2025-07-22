FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/ApiGateway/FileStorage.ApiGateway/FileStorage.ApiGateway.csproj", "src/ApiGateway/FileStorage.ApiGateway/"]
RUN dotnet restore "src/ApiGateway/FileStorage.ApiGateway/FileStorage.ApiGateway.csproj"
COPY .
WORKDIR "/src/src/ApiGateway/FileStorage.ApiGateway"
RUN dotnet build "FileStorage.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileStorage.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileStorage.ApiGateway.dll"]
