## DataBase Management

Set AuthApi project as Startup Project

Open "Package Manager Console" select the projeto "AuthApi" as option of "Default project"

Command to add new migration
```bash
Add-Migration NewMigrationName -Project Identity -Context Identity.IdentityDbContext.IdentityDbContext -OutputDir IdentityDbContext/Migrations
```
Apply migrations to database.
```bash
Update-Database -Project Identity -Context Identity.IdentityDbContext.IdentityDbContext
```

Undo migration steps.
```bash
Update-Database OldMigrationName -Project Identity -Context Identity.IdentityDbContext.IdentityDbContext
Remove-Migration -Project Identity -Context Identity.IdentityDbContext.IdentityDbContext
```

Documentação das [migrations.](https://docs.microsoft.com/pt-br/ef/core/managing-schemas/migrations/)
