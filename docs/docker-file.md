
## SQL Server Instance

```powershell

$sa_password = "Pass@word123"

docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=$sa_password" -p 1433:1433 -v sqlvolume:/var/opt/mssql -d --rm --name mssql mcr.microsoft.com/mssql/server:2022-latest

```

## Criando a Imagem

Step 1. Na pasta onde esta definido o `docker-compose.yml`, ou seja, na pasta raiz da solution `Task-Manager` rodar o seguinte commando:

```powershell

docker build -t task-manager-api-development:1.0 . -f src\Bigai.TaskManager.Api\Dockerfile

```

## Verificando a Imagem

Step 2. Depois de criar a imagem, você pode verificar se ela foi criada com o nome e a tag corretos utilizando comando a seguir:

```powershell

docker images

```

Será listado assim:

```powershell

REPOSITORY                     TAG       IMAGE ID       CREATED              SIZE
task-manager-api-development   1.0       d09737f97702   About a minute ago   349MB

```

## Rodando a Imagem

Step 3. Para rodar a imagem você pode utilizar um dos seguintes comenados:

```powershell

docker run -p 5000:5000 task-manager-api-development:1.0

```

## Verificando se a Imagem Esta Rodando

Step 4. Para verifiar se a imagem está rodando utilize o seguinte commando:

```powershell

docker ps

```

## Excluindo a Imagem

Para excluir a imagem utilize o seguinte commando:

```powershell

docker rmi $(docker images -a -q)

```
