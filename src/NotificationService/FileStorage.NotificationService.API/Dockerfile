FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/NotificationService/FileStorage.NotificationService.API/FileStorage.NotificationService.API.csproj", "src/NotificationService/FileStorage.NotificationService.API/"]
COPY ["src/FileStorage.Shared/FileStorage.Shared.csproj", "src/FileStorage.Shared/"]
RUN dotnet restore "src/NotificationService/FileStorage.NotificationService.API/FileStorage.NotificationService.API.csproj"
COPY .
WORKDIR "/src/src/NotificationService/FileStorage.NotificationService.API"
RUN dotnet build "FileStorage.NotificationService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileStorage.NotificationService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileStorage.NotificationService.API.dll"]
