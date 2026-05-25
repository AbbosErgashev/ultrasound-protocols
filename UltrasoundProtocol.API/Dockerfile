FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["UltrasoundProtocol.API/UltrasoundProtocol.API.csproj", "UltrasoundProtocol.API/"]
COPY ["UltrasoundProtocol.Application/UltrasoundProtocol.Application.csproj", "UltrasoundProtocol.Application/"]
COPY ["UltrasoundProtocol.Infrastructure/UltrasoundProtocol.Infrastructure.csproj", "UltrasoundProtocol.Infrastructure/"]
COPY ["UltrasoundProtocol.Domain/UltrasoundProtocol.Domain.csproj", "UltrasoundProtocol.Domain/"]

RUN dotnet restore "./UltrasoundProtocol.API/UltrasoundProtocol.API.csproj"

COPY . .

WORKDIR "/src/UltrasoundProtocol.API"

RUN dotnet build "./UltrasoundProtocol.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "./UltrasoundProtocol.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "UltrasoundProtocol.API.dll"]