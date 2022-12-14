# How to:

1. Open: "Package Manager Console"
2. Run: dotnet ef migrations add initial -o Database\Migrations --project .\src\Server\Drogecode.Knrm.Oefenrooster.Server.csproj
3. Run: dotnet ef database update  -o Database\Migrations --project .\src\Server\Drogecode.Knrm.Oefenrooster.Server.csproj