FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/IdentityService/FileStorage.IdentityService.API/FileStorage.IdentityService.API.csproj", "src/IdentityService/FileStorage.IdentityService.API/"]
COPY ["src/IdentityService/FileStorage.IdentityService.Application/FileStorage.IdentityService.Application.csproj", "src/IdentityService/FileStorage.IdentityService.Application/"]
COPY ["src/IdentityService/FileStorage.IdentityService.Domain/FileStorage.IdentityService.Domain.csproj", "src/IdentityService/FileStorage.IdentityService.Domain/"]
COPY ["src/IdentityService/FileStorage.IdentityService.Infrastructure/FileStorage.IdentityService.Infrastructure.csproj", "src/IdentityService/FileStorage.IdentityService.Infrastructure/"]
COPY ["src/FileStorage.Shared/FileStorage.Shared.csproj", "src/FileStorage.Shared/"]
RUN dotnet restore "src/IdentityService/FileStorage.IdentityService.API/FileStorage.IdentityService.API.csproj"
COPY .
WORKDIR "/src/src/IdentityService/FileStorage.IdentityService.API"
RUN dotnet build "FileStorage.IdentityService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileStorage.IdentityService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileStorage.IdentityService.API.dll"]
