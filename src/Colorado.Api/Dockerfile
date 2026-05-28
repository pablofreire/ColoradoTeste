# Build multi-stage para reduzir o tamanho da imagem final.
# O contexto do build e a pasta src/ (ver docker-compose.yml).
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copia primeiro os .csproj para aproveitar o cache de restore.
COPY ["Colorado.Api/Colorado.Api.csproj", "Colorado.Api/"]
COPY ["Colorado.Aplicacao/Colorado.Aplicacao.csproj", "Colorado.Aplicacao/"]
COPY ["Colorado.Dominio/Colorado.Dominio.csproj", "Colorado.Dominio/"]
COPY ["Colorado.Infraestrutura/Colorado.Infraestrutura.csproj", "Colorado.Infraestrutura/"]
RUN dotnet restore "Colorado.Api/Colorado.Api.csproj"

COPY . .
RUN dotnet publish "Colorado.Api/Colorado.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Usuario nao-root por seguranca.
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

EXPOSE 80
ENTRYPOINT ["dotnet", "Colorado.Api.dll"]
