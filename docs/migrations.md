# Migrations

- [Migrations](#migrations)
  - [Generating Migrations](#generating-migrations)
  - [Applying Migrations](#applying-migrations)
  - [Removing Migrations](#removing-migrations)

## Generating Migrations

```powershell

dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0

dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0

```

```powershell

dotnet ef migrations add InitialCreate --project src\Bigai.TaskManager.Infrastructure\Bigai.TaskManager.Infrastructure.csproj --startup-project src\Bigai.TaskManager.Api\Bigai.TaskManager.Api.csproj --output-dir Data\Migrations

```

[Back to top](#migrations)

## Applying Migrations

```powershell

dotnet ef database update --project src\Bigai.TaskManager.Infrastructure\Bigai.TaskManager.Infrastructure.csproj --startup-project src\Bigai.TaskManager.Api\Bigai.TaskManager.Api.csproj

```

[Back to top](#migrations)

## Removing Migrations

```powershell

dotnet ef migrations remove --project src\Bigai.TaskManager.Infrastructure\Bigai.TaskManager.Infrastructure.csproj --startup-project src\Bigai.TaskManager.Api\Bigai.TaskManager.Api.csproj

```

[Back to top](#migrations)