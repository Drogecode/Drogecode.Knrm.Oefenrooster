#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Drogecode.Knrm.Oefenrooster/Server/Drogecode.Knrm.Oefenrooster.Server.csproj", "Drogecode.Knrm.Oefenrooster/Server/"]
COPY ["Drogecode.Knrm.Oefenrooster/Client/Drogecode.Knrm.Oefenrooster.Client.csproj", "Drogecode.Knrm.Oefenrooster/Client/"]
COPY ["Drogecode.Knrm.Oefenrooster/Shared/Drogecode.Knrm.Oefenrooster.Shared.csproj", "Drogecode.Knrm.Oefenrooster/Shared/"]
RUN dotnet restore "Drogecode.Knrm.Oefenrooster/Server/Drogecode.Knrm.Oefenrooster.Server.csproj"
COPY . .
WORKDIR "/src/Drogecode.Knrm.Oefenrooster/Server"
RUN dotnet build "Drogecode.Knrm.Oefenrooster.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Drogecode.Knrm.Oefenrooster.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Drogecode.Knrm.Oefenrooster.Server.dll"]