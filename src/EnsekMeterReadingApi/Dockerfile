# Use the official .NET 9 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src/EnsekMeterReadingApi

# Copy csproj files and restore as distinct layers for better build caching
COPY EnsekMeterReadingApi.Api/EnsekMeterReadingApi.Api.csproj EnsekMeterReadingApi.Api/
COPY EnsekMeterReadingApi.Core/EnsekMeterReadingApi.Core.csproj EnsekMeterReadingApi.Core/
COPY EnsekMeterReadingApi.Infrastructure/EnsekMeterReadingApi.Infrastructure.csproj EnsekMeterReadingApi.Infrastructure/
RUN dotnet restore EnsekMeterReadingApi.Api/EnsekMeterReadingApi.Api.csproj

# Copy the rest of the source code
COPY EnsekMeterReadingApi.Api/ EnsekMeterReadingApi.Api/
COPY EnsekMeterReadingApi.Core/ EnsekMeterReadingApi.Core/
COPY EnsekMeterReadingApi.Infrastructure/ EnsekMeterReadingApi.Infrastructure/

WORKDIR /src/EnsekMeterReadingApi/EnsekMeterReadingApi.Api
RUN dotnet build EnsekMeterReadingApi.Api.csproj -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish EnsekMeterReadingApi.Api.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Use the official .NET 9 ASP.NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "EnsekMeterReadingApi.Api.dll"]