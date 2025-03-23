FROM ghcr.io/estudiogallo/net-core:0.1 AS build

WORKDIR /app
COPY . .
RUN dotnet restore

WORKDIR "/app/src/WebApi"
RUN dotnet build "WebApi.csproj" -c Release -o /app/out

FROM build AS publish
RUN dotnet publish "WebApi.csproj" -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/out .
ENTRYPOINT ["dotnet", "WebApi.dll"]
