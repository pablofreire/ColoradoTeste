# Colorado - Cadastro de Clientes

[English](README.md) · **Português**

Solucao para o desafio de Desenvolvedor Fullstack do Grupo Colorado: um CRUD de clientes
em ASP.NET Core (.NET 6) com duas camadas de apresentacao - uma Web API RESTful e um
Web App MVC - sobre uma base de Clean Architecture com padrao de repositorio e Unit of Work.

## Sobre o projeto

O dominio e composto por tres entidades:

- **Cliente** - dados cadastrais e de endereco, com tipo de pessoa (Fisica/Juridica) e documento: CPF, ou CNPJ inclusive no **formato alfanumerico** (nova regra da Receita, jul/2026; o CNPJ numerico antigo continua valido).
- **Telefone** - telefones do cliente (chave composta cliente + numero), com tipo e operadora.
- **TipoTelefone** - RESIDENCIAL, COMERCIAL, WHATSAPP, CELULAR (carga inicial via seed).

O **Web App e um frontend puro**: ele nao acessa o banco e consome a **API REST por HTTP**.
A API e a unica dona da persistencia (DbContext, repositorios, migrations). No Web, um cliente
HTTP generico (`ApiHttpCliente`) concentra serializacao, envelope e tratamento de erro, e os
clientes finos por entidade (`ClienteApiClient`, `TipoTelefoneApiClient`) apenas conhecem as
rotas. O endereco da API vem da configuracao `ApiBaseUrl`.

Validacao em duas frentes: no **front**, o formulario valida **inline** (Bootstrap
`is-invalid`/`invalid-feedback`) antes de enviar, **sem postback**, espelhando as regras; no
**servidor**, **FluentValidation** nos serviços de aplicacao (executados na API) mais as
invariantes de dominio garantem a regra. Em erro, a API responde 400 com a lista de erros e o
Web devolve **JSON** que o JS exibe num box dentro do modal aberto (sem reconstruir o formulario).

No Web App, o AJAX trabalha com **partial views renderizadas no servidor** (tabela e
formulario) e **ViewModels** proprios da camada de apresentacao (mapeados de/para os DTOs da
Aplicacao). Os artefatos da Web sao organizados por feature, espelhando a pasta da View
(ex.: `Views/Cliente/`, `Models/Cliente/`, `Mapeamentos/Cliente/`, `wwwroot/js/Cliente/`).

O formulario de cliente e **dinamico conforme o tipo de pessoa** (combo no topo do form): para
**Juridica** mostra "Razao social" + "Nome fantasia" e o documento e **CNPJ** (inclusive
alfanumerico); para **Fisica** o primeiro campo vira "Nome completo", o "Nome fantasia" fica
**oculto e dispensado** da validacao, e o documento e **CPF**. As **mascaras de input (IMask)**
acompanham o tipo (CPF/CNPJ/CEP/telefone), a **UF e um combo** com os 27 estados, e o documento
aparece **formatado tambem na listagem**. Na **edicao**, o "Tipo de pessoa" fica **read-only** e o
**"Usuario que cadastrou"** (gravado em `UsuarioInsercao`) e exibido somente leitura - esse campo
so e solicitado na **criacao**. As mascaras sao apenas visuais; o dominio normaliza os valores ao
persistir.

## Pre-requisitos

- .NET SDK 6.0 ou superior
- (Opcional) Docker + Docker Compose, para subir tudo com SQL Server
- (Opcional) ferramenta `dotnet-ef`: `dotnet tool install --global dotnet-ef --version 6.*`

## Banco de dados

A persistencia e **SQL Server** (apenas a API acessa o banco). A connection string fica em
`ConnectionStrings:ConexaoPadrao` e o registro do `DbContext`/repositorios/UoW fica isolado em
`InfraestruturaServiceCollectionExtensions.AdicionarInfraestrutura`. As migrations sao aplicadas
no startup da API (com retry enquanto o SQL Server fica pronto).

Para dev local e preciso um SQL Server acessivel - o jeito mais simples e subir so o banco do
compose: `docker compose up -d sqlserver`. O compose publica o SQL em **`localhost,1434`** (porta
interna 1433 mapeada para 1434 no host, evitando conflito com uma instancia local na 1433).

Nenhuma senha fica versionada. Copie `.env.example` para `.env` e defina `MSSQL_SA_PASSWORD`
(o compose le esse arquivo). Para rodar a API fora do Docker, grave a connection string em
user-secrets:

```
dotnet user-secrets set "ConnectionStrings:ConexaoPadrao" "Server=localhost,1434;Database=ColoradoDb;User Id=sa;Password=<sua MSSQL_SA_PASSWORD>;TrustServerCertificate=True;" --project src/Colorado.Api
```

## Como rodar (local)

Na raiz da solucao (`ColoradoTeste.sln`):

```
dotnet restore
dotnet build
```

Suba um SQL Server (ex.: `docker compose up -d sqlserver`) e rode a API (Swagger em `/swagger`):

```
dotnet run --project src/Colorado.Api
```

Web App MVC (consome a API; ajuste `ApiBaseUrl` se a API nao estiver na URL configurada):

```
dotnet run --project src/Colorado.Web
```

### Rodar no Visual Studio

1. Suba apenas o banco: `docker compose up -d sqlserver` (SQL em `localhost,1434`).
2. Rode a **API** pelo perfil Kestrel `Colorado.Api` (escuta em `http://localhost:5189`; Swagger em `/swagger`).
3. Rode a **Web**; o `appsettings.Development.json` ja aponta `ApiBaseUrl=http://localhost:5189`.

Dica: configure "Multiple startup projects" (Solution > Properties) com **Colorado.Api** e
**Colorado.Web** como *Start* para subir as duas de uma vez. Em container, `ApiBaseUrl` e a
connection string vem por variavel de ambiente (precedencia sobre os `appsettings`), entao o
fluxo Docker nao e afetado por essas portas de dev.

Para gerar/aplicar migrations manualmente (precisa de um SQL Server acessivel):

```
export ConnectionStrings__ConexaoPadrao="Server=localhost,1434;Database=ColoradoDb;User Id=sa;Password=<sua MSSQL_SA_PASSWORD>;TrustServerCertificate=True;"
dotnet ef database update --project src/Colorado.Infraestrutura
```

## Como rodar com Docker (SQL Server)

Na raiz do repositorio, com Docker em execucao e o `.env` criado a partir do `.env.example`:

```
docker compose up --build
```

Sobe tres containers: `sqlserver`, `api` (unico que acessa o banco) e `web` (consome a `api`
via `ApiBaseUrl=http://api:80`). Na rede do compose a API fala com o SQL pela porta interna
`1433`; o host expoe o SQL em `1434` (so para VS/SSMS). Apos subir:

- API + Swagger: http://localhost:5000/swagger
- Web App MVC: http://localhost:5002

As migrations sao aplicadas no startup da API (com retry enquanto o SQL Server fica pronto).

## Testes

Testes automatizados (xUnit + NSubstitute + FluentAssertions), um projeto por camada sob
`tests/`:

- `Colorado.Dominio.Testes` - entidades (invariantes, comportamento) e validacao de CPF/CNPJ.
- `Colorado.Aplicacao.Testes` - validators FluentValidation, mapeadores e fluxos dos serviços.
- `Colorado.Infraestrutura.Testes` - repositorios sobre SQLite in-memory (filtros, paginacao,
  include, chave composta e seed). O SQLite e usado **apenas nos testes**; em producao o banco e
  SQL Server.
- `Colorado.Api.Testes` - controllers da API com os serviços mockados.
- `Colorado.Web.Testes` - controllers do MVC com os clientes de API (`*ApiClient`) mockados.

```
dotnet test
```

## Estrutura

No `.sln` os projetos sao agrupados por camada (pastas de solution `01-Presentation`,
`02-Application`, `03-Domain`, `04-Infrastructure`, `Tests`). No disco:

```
src/
  Colorado.Dominio/         Entidades, enums, validacoes, interfaces e excecoes de dominio
  Colorado.Aplicacao/       Servicos, DTOs, mapeamentos manuais, validators e interfaces
  Colorado.Infraestrutura/  DbContext, configuracoes Fluent API, repositorios, UoW e migrations
  Colorado.Api/             Web API RESTful + Swagger + middleware de excecao
  Colorado.Web/             Web App MVC (Razor + Bootstrap 5 + JS puro) + ApiClients (consome a API)
```

Dentro de cada camada, os artefatos especificos de cada entidade ficam em subpastas por feature
(`Clientes/`, `TiposTelefone/`), com o que e compartilhado (base de repositorio, UoW, interfaces
genericas, excecoes) na raiz da pasta. Ex.: `Repositorios/RepositorioBase.cs` e
`Repositorios/UnitOfWork.cs` na raiz; `Repositorios/Clientes/ClienteRepositorio.cs` na subpasta.

```
tests/
  Colorado.Dominio.Testes/        Testes de entidades e validacoes
  Colorado.Aplicacao.Testes/      Testes de validators, mapeadores e serviços
  Colorado.Infraestrutura.Testes/ Testes de repositorios (SQLite in-memory)
  Colorado.Api.Testes/            Testes de controllers da API
  Colorado.Web.Testes/            Testes de controllers do MVC
```

## Endpoints da API

Clientes:

- `GET    /api/clientes` - lista paginada, com filtros `nome` e `documento`
- `GET    /api/clientes/{codigo}` - obtem o cliente com seus telefones
- `POST   /api/clientes` - cria
- `PUT    /api/clientes/{codigo}` - atualiza
- `DELETE /api/clientes/{codigo}` - remove

Tipos de telefone:

- `GET    /api/tipos-telefone`
- `GET    /api/tipos-telefone/{codigo}`
- `POST   /api/tipos-telefone`
- `PUT    /api/tipos-telefone/{codigo}`
- `DELETE /api/tipos-telefone/{codigo}`

Todas as respostas usam um envelope padrao (`sucesso`, `mensagem`, `dados`, `erros`) e os
status HTTP adequados (200, 201, 204, 400, 404, 500). Falhas de validacao e regra de negocio
retornam 400 (com a lista de erros), recursos inexistentes 404 e excecoes nao tratadas 500,
sempre via middleware.

## Observacoes finais

Optei por nao usar AutoMapper: para um CRUD desse tamanho o mapeamento manual fica mais
explicito, mais facil de depurar e tem zero overhead. Pelo mesmo motivo nao adotei MediatR -
os servicos de aplicacao resolvem bem o fluxo sem a indirecao.

A validacao de input usa FluentValidation nos serviços, reaproveitando o validador de CPF/CNPJ
do dominio (digitos verificadores). O `EhCnpjValido` ja trata o **CNPJ alfanumerico**: cada
caractere vale `ASCII - 48` no calculo do modulo 11 ('0'-'9' = 0-9, 'A'-'Z' = 17-42) e
`Documento.Normalizar` preserva letras (em maiusculas); o mesmo algoritmo e espelhado no
JavaScript. O front repete a checagem apenas para dar retorno imediato ao usuario. A persistencia e SQL Server (o `docker-compose` sobe o banco junto
da API); nos testes de repositorio uso SQLite in-memory para rodar rapido e sem dependencia
externa, mantendo a verificacao de constraints relacionais.
