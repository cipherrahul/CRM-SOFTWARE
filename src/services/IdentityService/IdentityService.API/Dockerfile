FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/services/IdentityService/IdentityService.API/IdentityService.API.csproj", "src/services/IdentityService/IdentityService.API/"]
COPY ["src/services/IdentityService/IdentityService.Application/IdentityService.Application.csproj", "src/services/IdentityService/IdentityService.Application/"]
COPY ["src/services/IdentityService/IdentityService.Infrastructure/IdentityService.Infrastructure.csproj", "src/services/IdentityService/IdentityService.Infrastructure/"]
COPY ["src/services/IdentityService/IdentityService.Domain/IdentityService.Domain.csproj", "src/services/IdentityService/IdentityService.Domain/"]
COPY ["src/shared/CRM.Shared/CRM.Shared.csproj", "src/shared/CRM.Shared/"]

RUN dotnet restore "./src/services/IdentityService/IdentityService.API/IdentityService.API.csproj"
COPY . .
WORKDIR "/src/src/services/IdentityService/IdentityService.API"
RUN dotnet build "./IdentityService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IdentityService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.API.dll"]
