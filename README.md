# Task-Manager

Representa a solução final para o [desafio](https://meteor-ocelot-f0d.notion.site/NET-C-5281edbec2e4480d98552e5ca0242c5b) de construção de um sistema de gerenciamento de tarefas, que permite aos usuários organiza e monitorar suas tarefas.

- [Task-Manager](#task-manager)
  - [Ambiente de Desenvolvimento](#ambiente-de-desenvolvimento)
  - [Authorization](#authorization)
    - [Gerando Manager Token](#gerando-manager-token)
    - [Gerando User Token](#gerando-user-token)
  - [Levantar a Aplicação](#levantar-a-aplicação)
    - [Passo 1 - Clonar o projeto](#passo-1---clonar-o-projeto)
    - [Passo 2 - Build do projeto](#passo-2---build-do-projeto)
    - [Passo 3 - Criar a Imagem](#passo-3---criar-a-imagem)
    - [Passo 4 - Verificando a Imagem](#passo-4---verificando-a-imagem)
    - [Passo 5 - Rodar o docker-compose](#passo-5---rodar-o-docker-compose)
  - [Suporte para Testes](#suporte-para-testes)

## Ambiente de Desenvolvimento

| TOOL                                                                                                                   | DESCRIPTION                                  |
| :--------------------------------------------------------------------------------------------------------------------- | :------------------------------------------- |
| [.NET SDK 7.0](https://dotnet.microsoft.com/pt-br/download/dotnet/7.0)                                                 | Para construir e executar aplicativos dotnet |
| [Docker Desktop](https://docs.docker.com/get-docker)                                                                   | Orquestrar e levantar a infraestrutura       |
| [Visual Studio Code](https://aka.ms/vscode)                                                                            | IDE de desenvolvimento                       |
| [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads?msockid=02916a913aad6e232c2779713b746f9b) | Persistência de dados                        |

## Authorization

Todos os endpoins da solução final estão protegidos por um `access token`.

Importante: Foi utilizada uma proteção por token disponibilizada pelo `dotnet sdk`, e ele só funciona durante a fase de desenvolvimento. Para gerar um token que permita o acesso aos endpoints utilize o comando a seguir:

### Gerando Manager Token

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

### Gerando User Token

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

## Levantar a Aplicação

A aplicação pode ser executa utilizando o Visual Studio Code, onde será possível ter acesso ao swagger e também rodar os testes unitários e integrado. Nesta seção estão descritos os passos que devem ser seguidos para rodar a aplicação no docker.

### Passo 1 - Clonar o projeto

Para clonar o projeto, no `powershell` da sua máquina, execute o comando `https://github.com/marcos-cruz/Task-Manager.git`

### Passo 2 - Build do projeto

Você pode utilizar a IDE de sua preferência para fazer o build do projeto.

Se você estiver confortável, execute o comando a seguir no seu powershell.

```powershell

dotnet build

```

### Passo 3 - Criar a Imagem

Na pasta onde esta definido o `docker-compose.yml`, ou seja, na pasta raiz da solution `Task-Manager` execute o seguinte commando:

```powershell

docker build -t task-manager-api-development:1.0 . -f src\Bigai.TaskManager.Api\Dockerfile

```

Importante: Não pule este comando, e copie ele exatamente como está, pois a imagem produzida será utilizada no `docker-compose` para fazer a orquestração.

### Passo 4 - Verificando a Imagem

Depois de criar a imagem, você pode verificar se ela foi criada com o nome e a tag corretos utilizando comando a seguir:

```powershell

docker images

```

Deve listado assim:

```powershell

REPOSITORY                     TAG       IMAGE ID       CREATED              SIZE
task-manager-api-development   1.0       d09737f97702   About a minute ago   349MB

```

Importante: Não tente rodar esta imagem, pois não temos o banco de dados rodando, e isto vai gerar um erro.

### Passo 5 - Rodar o docker-compose

Ainda na pasta onde esta definido o `docker-compose.yml`, ou seja, na pasta raiz da solution `Task-Manager` execute o seguinte commando:

```powershell

docker-compose -f docker-compose.yml up --build

```

Pronto! A aplicação está rodando.

Para parar aplicação digite `Ctrl+C` e em seguida execute o comando `docker-compose stop` ou `docker-compose down`

## Suporte para Testes

Na pasta `Bigai.TaskManager.Api` estão disponibilizados os arquivos `Bigai.TaskManager.ProjectApi.http`, `Bigai.TaskManager.WorkUnitApi.http` e `Bigai.TaskManager.PerformanceApi.http` os podem auxiliar nos testes unitários e integrado com um feedback mais amigável.
