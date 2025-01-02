# How to:

1. Open: "Package Manager Console"
2. cd .\src\Server
3. dotnet ef migrations add AuditLogins -o Database/Migrations --project ./Drogecode.Knrm.Oefenrooster.Server.csproj
4. dotnet ef database update --project ./Drogecode.Knrm.Oefenrooster.Server.csproj

# Revert

dotnet ef database update "20241229225208_AddDbReportActionShared"

# Update tools:

dotnet tool update --global dotnet-ef --version on 8.0.11

# Create script:

dotnet ef migrations script

## AddLinkUserRole as your most recent migration

dotnet ef migrations script AddLinkUserRole -o OutputScript.sql