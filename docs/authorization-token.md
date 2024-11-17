# Authorization

- [Authorization](#authorization)
  - [Gerando Manager Token](#gerando-manager-token)
  - [Gerando User Token](#gerando-user-token)

## Gerando Manager Token

```powershell

dotnet user-jwts create --project src\Bigai.TaskManager.Api\Bigai.TaskManager.Api.csproj --role "Manager" --claim "userId=365" --name="TaskManager" --valid-for 365d

```

gerado um token, valido somente para `Manager`, e em localhost, conforme abaixo:

```powershell

New JWT saved with ID '24682fb'.
Name: TaskManager
Expires On: 2025-11-15T18:04:57.7106933Z
Roles: [Manager]
Custom Claims: [userId=7]

Token: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IlRhc2tNYW5hZ2VyIiwic3ViIjoiVGFza01hbmFnZXIiLCJqdGkiOiIyNDY4MmZiIiwicm9sZSI6Ik1hbmFnZXIiLCJ1c2VySWQiOiI3IiwiYXVkIjpbImh0dHA6Ly9sb2NhbGhvc3Q6NDQxMSIsImh0dHBzOi8vbG9jYWxob3N0OjQ0MzUyIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMSIsImh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCJdLCJuYmYiOjE3MzE2OTM4OTcsImV4cCI6MTc2MzIyOTg5NywiaWF0IjoxNzMxNjkzOTAwLCJpc3MiOiJkb3RuZXQtdXNlci1qd3RzIn0.PFVlGyBAu9oa9DAXD6o_E8fnOSG86nGyxIzTayEhfRU

```

[Ir para o topo](#authorization)

## Gerando User Token

```powershell

dotnet user-jwts create --project src\Bigai.TaskManager.Api\Bigai.TaskManager.Api.csproj --role "User" --claim "userId=462" --name="TaskManager" --valid-for 365d

```

Novamente será gerado um token, valido somente para `User`, e em localhost, conforme abaixo:

```powershell

New JWT saved with ID '77c24cc9'.
Name: TaskManager
Expires On: 2025-11-15T18:15:44.3606508Z
Roles: [User]
Custom Claims: [userId=462]

Token: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IlRhc2tNYW5hZ2VyIiwic3ViIjoiVGFza01hbmFnZXIiLCJqdGkiOiI3N2MyNGNjOSIsInJvbGUiOiJVc2VyIiwidXNlcklkIjoiNDYyIiwiYXVkIjpbImh0dHA6Ly9sb2NhbGhvc3Q6NDQxMSIsImh0dHBzOi8vbG9jYWxob3N0OjQ0MzUyIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMSIsImh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCJdLCJuYmYiOjE3MzE2OTQ1NDQsImV4cCI6MTc2MzIzMDU0NCwiaWF0IjoxNzMxNjk0NTQ3LCJpc3MiOiJkb3RuZXQtdXNlci1qd3RzIn0.sHQC7gqtY1_RCCo8v4H6b6nBj0iYzuIM5bjoOkvdZUo

```

[Ir para o topo](#authorization)