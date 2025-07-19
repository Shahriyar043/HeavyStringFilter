# --------------------------
# BUILD STAGE
# --------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.sln ./
COPY src/HeavyStringFilter.Api/HeavyStringFilter.Api.csproj ./src/HeavyStringFilter.Api/
COPY src/HeavyStringFilter.Application/HeavyStringFilter.Application.csproj ./src/HeavyStringFilter.Application/
COPY src/HeavyStringFilter.Infrastructure/HeavyStringFilter.Infrastructure.csproj ./src/HeavyStringFilter.Infrastructure/
COPY tests/HeavyStringFilter.Tests/HeavyStringFilter.Tests.csproj ./tests/HeavyStringFilter.Tests/
COPY tests/HeavyStringFilter.IntegrationTests/HeavyStringFilter.IntegrationTests.csproj ./tests/HeavyStringFilter.IntegrationTests/
COPY tests/HeavyStringFilter.PerformanceTests/HeavyStringFilter.PerformanceTests.csproj ./tests/HeavyStringFilter.PerformanceTests/
COPY tests/TestCommon/TestCommon.csproj ./tests/TestCommon/

RUN dotnet restore

COPY . .
WORKDIR /src/src/HeavyStringFilter.Api
RUN dotnet publish -c Release -o /app/publish

# --------------------------
# RUNTIME STAGE
# --------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "HeavyStringFilter.Api.dll"]
