#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#base stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

#build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# Install NodeJS
RUN set -uex; \
    apt-get update; \
    apt-get install -y ca-certificates curl gnupg; \
    mkdir -p /etc/apt/keyrings; \
    curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key \
     | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg; \
    NODE_MAJOR=22; \
    echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_$NODE_MAJOR.x nodistro main" \
     > /etc/apt/sources.list.d/nodesource.list; \
    apt-get -qy update; \
    apt-get -qy install nodejs;

WORKDIR /src
COPY ["src/Server/Drogecode.Knrm.Oefenrooster.Server.csproj", "Server/"]
RUN dotnet restore "Server/Drogecode.Knrm.Oefenrooster.Server.csproj"
COPY . .
WORKDIR "/src/src/Server"
RUN dotnet build "Drogecode.Knrm.Oefenrooster.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Drogecode.Knrm.Oefenrooster.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
HEALTHCHECK --interval=1m --timeout=3s --start-period=60s --retries=3 \
  CMD curl -f http://localhost/api/_health || exit 1
ENTRYPOINT ["dotnet", "Drogecode.Knrm.Oefenrooster.Server.dll"]