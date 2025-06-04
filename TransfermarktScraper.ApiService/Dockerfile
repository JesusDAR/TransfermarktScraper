# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY ["TransfermarktScraper.ApiService/TransfermarktScraper.ApiService.csproj", "TransfermarktScraper.ApiService/"]
COPY ["TransfermarktScraper.BLL/TransfermarktScraper.BLL.csproj", "TransfermarktScraper.BLL/"]
COPY ["TransfermarktScraper.Data/TransfermarktScraper.Data.csproj", "TransfermarktScraper.Data/"]
COPY ["TransfermarktScraper.Domain/TransfermarktScraper.Domain.csproj", "TransfermarktScraper.Domain/"]
COPY ["TransfermarktScraper.ServiceDefaults/TransfermarktScraper.ServiceDefaults.csproj", "TransfermarktScraper.ServiceDefaults/"]
RUN dotnet restore "./TransfermarktScraper.ApiService/TransfermarktScraper.ApiService.csproj"
COPY . .
WORKDIR "/src/TransfermarktScraper.ApiService"
RUN dotnet build "./TransfermarktScraper.ApiService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TransfermarktScraper.ApiService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .

USER root
RUN apt-get update && \
    apt-get install -y wget apt-transport-https software-properties-common && \
    wget -q https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y powershell && \
    rm packages-microsoft-prod.deb

RUN pwsh playwright.ps1 install chromium
RUN pwsh playwright.ps1 install-deps chromium
RUN rm -rf playwright.ps1
RUN apt remove wget powershell -yq

ENTRYPOINT ["dotnet", "TransfermarktScraper.ApiService.dll"]