# How to:

1. Open: "Package Manager Console"
2. cd ..\Server
3. dotnet ef migrations add AddLicenses -o Database/Migrations --project ./Drogecode.Knrm.Oefenrooster.Server.csproj
4. dotnet ef database update --project ./Drogecode.Knrm.Oefenrooster.Server.csproj

# Revert

dotnet ef database update "20250719101736_IncreaseTrainingDescriptionLength"

# Update tools:

dotnet tool update --global dotnet-ef --version 9.0.7

# Create a script:

dotnet ef migrations script

## AddLinkUserRole as your most recent migration

dotnet ef migrations script AddLinkUserRole -o OutputScript.sql