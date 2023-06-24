build:
	dotnet build

clean:
	dotnet clean

restore:
	dotnet restore

watch:
	dotnet watch --project src/Web/Serenity.Web.csproj run --launch-profile https

run:
	dotnet run --project src/Web/Serenity.Web.csproj --launch-profile https

migration:
	dotnet ef migrations add "$(name)" --project src\Infrastructure --startup-project src\Web --output-dir Persistence\Migrations

dbupdate:
	dotnet ef database update --project src\Infrastructure --startup-project src\Web 