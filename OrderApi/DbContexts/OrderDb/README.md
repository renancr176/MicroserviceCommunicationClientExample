## DataBase Management

Set this project as Startup Project

Open "Package Manager Console" select the projeto "OrderApi" as option of "Default project"

Command to add new migration
```bash
Add-Migration NewMigrationName -Context OrderDbContext -OutputDir DbContexts/OrderDb/Migrations
```
Apply migrations to database.
```bash
Update-Database -Context OrderDbContext
```

Undo migration steps.
```bash
Update-Database OldMigrationName -Context OrderDbContext
Remove-Migration -Context OrderDbContext
```

Documentação das [migrations.](https://docs.microsoft.com/pt-br/ef/core/managing-schemas/migrations/)
