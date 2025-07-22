FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/FileService/FileStorage.FileService.API/FileStorage.FileService.API.csproj", "src/FileService/FileStorage.FileService.API/"]
COPY ["src/FileService/FileStorage.FileService.Application/FileStorage.FileService.Application.csproj", "src/FileService/FileStorage.FileService.Application/"]
COPY ["src/FileService/FileStorage.FileService.Domain/FileStorage.FileService.Domain.csproj", "src/FileService/FileStorage.FileService.Domain/"]
COPY ["src/FileService/FileStorage.FileService.Infrastructure/FileStorage.FileService.Infrastructure.csproj", "src/FileService/FileStorage.FileService.Infrastructure/"]
COPY ["src/FileStorage.Shared/FileStorage.Shared.csproj", "src/FileStorage.Shared/"]
RUN dotnet restore "src/FileService/FileStorage.FileService.API/FileStorage.FileService.API.csproj"
COPY .
WORKDIR "/src/src/FileService/FileStorage.FileService.API"
RUN dotnet build "FileStorage.FileService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileStorage.FileService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileStorage.FileService.API.dll"]
