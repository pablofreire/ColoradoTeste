# Colorado - Cadastro de Clientes

Solucao para o desafio de Desenvolvedor Fullstack do Grupo Colorado: um CRUD de clientes
em ASP.NET Core (.NET 6) com duas camadas de apresentacao - uma Web API RESTful e um
Web App MVC - sobre uma base de Clean Architecture com padrao de repositorio e Unit of Work.

## Sobre o projeto

O dominio e composto por tres entidades:

- **Cliente** - dados cadastrais e de endereco, com tipo de pessoa (Fisica/Juridica) e documento (CPF/CNPJ).
- **Telefone** - telefones do cliente (chave composta cliente + numero), com tipo e operadora.
- **TipoTelefone** - RESIDENCIAL, COMERCIAL, WHATSAPP, CELULAR (carga inicial via seed).

A API e o Web App sao camadas de apresentacao independentes que consomem a mesma camada de
Aplicacao. O Web App nao chama a API por HTTP: ele usa a Aplicacao diretamente, o que evita
um salto de rede desnecessario para uma aplicacao que roda no mesmo processo.

Validacao de input com **FluentValidation** nos serviços de aplicacao (vale para as duas
apresentacoes). Regras de negocio e invariantes ficam no dominio.

No Web App, o AJAX trabalha com **partial views renderizadas no servidor** (tabela e
formulario) e **ViewModels** proprios da camada de apresentacao (mapeados de/para os DTOs da
Aplicacao). Os artefatos da Web sao organizados por feature, espelhando a pasta da View
(ex.: `Views/Cliente/`, `Models/Cliente/`, `Mapeamentos/Cliente/`, `wwwroot/js/Cliente/`).

## Pre-requisitos

- .NET SDK 6.0 ou superior
- (Opcional) Docker + Docker Compose, para subir tudo com SQL Server
- (Opcional) ferramenta `dotnet-ef`: `dotnet tool install --global dotnet-ef --version 6.*`

## Banco de dados

O provider e escolhido pela chave de configuracao `DatabaseProvider`:

- **`Sqlite`** (padrao no dev local) - arquivo `colorado.db` criado no startup. Sem instalacao.
- **`SqlServer`** (usado no Docker) - migrations aplicadas no startup.

A connection string fica em `ConnectionStrings:ConexaoPadrao`. Tudo isolado em
`InfraestruturaServiceCollectionExtensions.AdicionarInfraestrutura`.

## Como rodar (local, SQLite)

Na raiz da solucao (`ColoradoTeste.sln`):

```
dotnet restore
dotnet build
```

API (Swagger em `/swagger`):

```
dotnet run --project src/Colorado.Api
```

Web App MVC:

```
dotnet run --project src/Colorado.Web
```

O banco e preparado automaticamente no startup. As migrations sao versionadas para SQL Server;
para gera-las/aplica-las manualmente (precisa de um SQL Server acessivel - veja Docker abaixo):

```
dotnet ef database update --project src/Colorado.Infraestrutura
```

## Como rodar com Docker (SQL Server)

Na raiz `Code/`, com Docker em execucao:

```
docker-compose up --build
```

Sobe tres containers: `sqlserver`, `api` e `web`, todos apontando para o mesmo banco
(compartilham os dados). Apos subir:

- API + Swagger: http://localhost:5000/swagger
- Web App MVC: http://localhost:5002

As migrations sao aplicadas no startup da aplicacao (com retry enquanto o SQL Server fica pronto).

## Testes

Testes automatizados (xUnit + NSubstitute + FluentAssertions), um projeto por camada sob
`tests/`:

- `Colorado.Dominio.Testes` - entidades (invariantes, comportamento) e validacao de CPF/CNPJ.
- `Colorado.Aplicacao.Testes` - validators FluentValidation, mapeadores e fluxos dos serviços.
- `Colorado.Infraestrutura.Testes` - repositorios sobre SQLite in-memory (filtros, paginacao,
  include, chave composta e seed).
- `Colorado.Api.Testes` / `Colorado.Web.Testes` - controllers com serviços mockados.

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
  Colorado.Web/             Web App MVC (Razor + Bootstrap 5 + JavaScript puro)
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
do dominio (digitos verificadores); o front repete a checagem em JavaScript apenas para dar
retorno imediato ao usuario. SQLite no dev local mantem o atrito baixo (zero instalacao) e o
Docker sobe SQL Server para um ambiente mais proximo de producao - a troca e so configuracao.
