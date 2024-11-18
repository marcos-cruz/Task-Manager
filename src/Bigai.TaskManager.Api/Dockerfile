FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

ENV ASPNETCORE_URLS=http://+:5000

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Development
WORKDIR /src
COPY ["src/Bigai.TaskManager.Api/Bigai.TaskManager.Api.csproj", "src/Bigai.TaskManager.Api/"]
RUN dotnet restore "src/Bigai.TaskManager.Api/Bigai.TaskManager.Api.csproj"
COPY . .
WORKDIR "/src/src/Bigai.TaskManager.Api"
RUN dotnet build "Bigai.TaskManager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG configuration=Development
RUN dotnet publish "Bigai.TaskManager.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bigai.TaskManager.Api.dll"]
