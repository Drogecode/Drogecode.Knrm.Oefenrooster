#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Server/Drogecode.Knrm.Oefenrooster.Server.csproj", "Server/"]
COPY ["src/Client/Drogecode.Knrm.Oefenrooster.Client.csproj", "Client/"]
COPY ["src/Shared/Drogecode.Knrm.Oefenrooster.Shared.csproj", "Shared/"]
RUN dotnet restore "Server/Drogecode.Knrm.Oefenrooster.Server.csproj"
COPY . .
WORKDIR "/src/src/Server"
RUN dotnet build "Drogecode.Knrm.Oefenrooster.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Drogecode.Knrm.Oefenrooster.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Drogecode.Knrm.Oefenrooster.Server.dll"]