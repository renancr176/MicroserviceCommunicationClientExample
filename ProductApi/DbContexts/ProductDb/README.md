## DataBase Management

Set this project as Startup Project

Open "Package Manager Console" select the projeto "CustomerApi" as option of "Default project"

Command to add new migration
```bash
Add-Migration NewMigrationName -Context ProductDbContext -OutputDir DbContexts/ProductDb/Migrations
```
Apply migrations to database.
```bash
Update-Database -Context ProductDbContext
```

Undo migration steps.
```bash
Update-Database OldMigrationName -Context ProductDbContext
Remove-Migration -Context ProductDbContext
```

Documentação das [migrations.](https://docs.microsoft.com/pt-br/ef/core/managing-schemas/migrations/)
