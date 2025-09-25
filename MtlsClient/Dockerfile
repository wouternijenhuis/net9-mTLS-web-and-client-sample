# Use the official .NET 9 runtime as a parent image
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

# Use the official .NET 9 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MtlsClient.csproj", "./"]
RUN dotnet restore "MtlsClient.csproj"
COPY . .
RUN dotnet build "MtlsClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MtlsClient.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ../certificates /app/certificates
ENTRYPOINT ["dotnet", "MtlsClient.dll"]