FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/gateway/CRM.Gateway/CRM.Gateway.csproj", "src/gateway/CRM.Gateway/"]
RUN dotnet restore "./src/gateway/CRM.Gateway/CRM.Gateway.csproj"
COPY . .
WORKDIR "/src/src/gateway/CRM.Gateway"
RUN dotnet build "./CRM.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CRM.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CRM.Gateway.dll"]
