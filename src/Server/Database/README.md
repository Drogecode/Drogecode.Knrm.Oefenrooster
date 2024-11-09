# How to:

1. Open: "Package Manager Console"
2. cd .\src\Server
3. dotnet ef migrations add AddLinkUserRole -o Database/Migrations --project ./Drogecode.Knrm.Oefenrooster.Server.csproj
4. dotnet ef database update --project ./Drogecode.Knrm.Oefenrooster.Server.csproj

# Revert

1. dotnet ef database update "20241002202511_UserLinkedMails-activation"

# Update tools:

1. dotnet tool update --global dotnet-ef --version 8.0.10