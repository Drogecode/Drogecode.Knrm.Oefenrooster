# How to:

1. Open: "Package Manager Console"
2. Run: dotnet ef migrations add initial --output-dir Database/Migrations --project ./src/Server/Drogecode.Knrm.Oefenrooster.Server.csproj
3. Run: dotnet ef database update --output-dir Database/Migrations --project ./Drogecode.Knrm.Oefenrooster.Server.csproj