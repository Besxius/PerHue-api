# ============================
# BASE RUNTIME IMAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ============================
# BUILD IMAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj trước để cache restore
COPY ["PerHue.Api/PerHue.Api.csproj", "PerHue.Api/"]
COPY ["PerHue.Application/PerHue.Application.csproj", "PerHue.Application/"]
COPY ["PerHue.Domain/PerHue.Domain.csproj", "PerHue.Domain/"]
COPY ["PerHue.Infrastructure/PerHue.Infrastructure.csproj", "PerHue.Infrastructure/"]

RUN dotnet restore "PerHue.Api/PerHue.Api.csproj"

# Copy toàn bộ solution
COPY . .

WORKDIR "/src/PerHue.Api"
RUN dotnet build "PerHue.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ============================
# PUBLISH IMAGE
# ============================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PerHue.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ============================
# FINAL RUNTIME IMAGE
# ============================
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "PerHue.Api.dll"]