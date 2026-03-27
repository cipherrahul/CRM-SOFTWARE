FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/services/AIService/AIService.API/AIService.API.csproj", "src/services/AIService/AIService.API/"]
COPY ["src/services/AIService/AIService.Application/AIService.Application.csproj", "src/services/AIService/AIService.Application/"]
COPY ["src/services/AIService/AIService.Infrastructure/AIService.Infrastructure.csproj", "src/services/AIService/AIService.Infrastructure/"]
COPY ["src/services/AIService/AIService.Domain/AIService.Domain.csproj", "src/services/AIService/AIService.Domain/"]
COPY ["src/shared/CRM.Shared/CRM.Shared.csproj", "src/shared/CRM.Shared/"]
COPY ["src/shared/CRM.Shared.Events/CRM.Shared.Events.csproj", "src/shared/CRM.Shared.Events/"]

RUN dotnet restore "./src/services/AIService/AIService.API/AIService.API.csproj"
COPY . .
WORKDIR "/src/src/services/AIService/AIService.API"
RUN dotnet build "./AIService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AIService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AIService.API.dll"]
