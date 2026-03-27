FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/services/CRMService/CRMService.API/CRMService.API.csproj", "src/services/CRMService/CRMService.API/"]
COPY ["src/services/CRMService/CRMService.Application/CRMService.Application.csproj", "src/services/CRMService/CRMService.Application/"]
COPY ["src/services/CRMService/CRMService.Infrastructure/CRMService.Infrastructure.csproj", "src/services/CRMService/CRMService.Infrastructure/"]
COPY ["src/services/CRMService/CRMService.Domain/CRMService.Domain.csproj", "src/services/CRMService/CRMService.Domain/"]
COPY ["src/shared/CRM.Shared/CRM.Shared.csproj", "src/shared/CRM.Shared/"]
COPY ["src/shared/CRM.Shared.Events/CRM.Shared.Events.csproj", "src/shared/CRM.Shared.Events/"]

RUN dotnet restore "./src/services/CRMService/CRMService.API/CRMService.API.csproj"
COPY . .
WORKDIR "/src/src/services/CRMService/CRMService.API"
RUN dotnet build "./CRMService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CRMService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CRMService.API.dll"]
