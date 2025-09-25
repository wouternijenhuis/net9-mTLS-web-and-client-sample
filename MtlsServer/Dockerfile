# Use the official .NET 9 runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the official .NET 9 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MtlsServer.csproj", "./"]
RUN dotnet restore "MtlsServer.csproj"
COPY . .
RUN dotnet build "MtlsServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MtlsServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ../certificates /app/certificates
ENTRYPOINT ["dotnet", "MtlsServer.dll"]