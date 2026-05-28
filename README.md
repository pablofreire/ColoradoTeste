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

## Pre-requisitos

- .NET SDK 6.0 ou superior
- (Opcional) ferramenta `dotnet-ef` para gerar/aplicar migrations manualmente:
  `dotnet tool install --global dotnet-ef --version 6.*`

O banco padrao e **SQLite** (arquivo `colorado.db`), criado automaticamente no primeiro
start de cada aplicacao. Nao e necessario instalar nenhum servidor de banco.

## Como rodar

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

As migrations sao aplicadas automaticamente no startup de cada aplicacao. Para recriar o
banco manualmente (os comandos `dotnet ef` rodam sobre o projeto de Infraestrutura, onde
ficam as migrations e a factory de design-time):

```
dotnet ef database drop -f --project src/Colorado.Infraestrutura
dotnet ef database update --project src/Colorado.Infraestrutura
```

## Estrutura de pastas

```
src/
  Colorado.Dominio/         Entidades, enums, validacoes, interfaces e excecoes de dominio
  Colorado.Aplicacao/       Servicos, DTOs, mapeamentos manuais e interfaces de servico
  Colorado.Infraestrutura/  DbContext, configuracoes Fluent API, repositorios, UoW e migrations
  Colorado.Api/             Web API RESTful + Swagger + middleware de excecao
  Colorado.Web/             Web App MVC (Razor + Bootstrap 5 + JavaScript puro)
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
status HTTP adequados (200, 201, 204, 400, 404, 500). Erros de regra de negocio retornam 400,
recursos inexistentes retornam 404 e excecoes nao tratadas retornam 500, sempre via middleware.

## Trocando o banco para SQL Server / LocalDB

A escolha do provedor fica isolada em `InfraestruturaServiceCollectionExtensions`. Para usar
LocalDB, troque `UseSqlite` por `UseSqlServer`, ajuste a connection string em
`appsettings.json` (ex.: `Server=(localdb)\\MSSQLLocalDB;Database=Colorado;Trusted_Connection=True;`)
e gere as migrations novamente para o novo provedor.

## Observacoes finais

Optei por nao usar AutoMapper: para um CRUD desse tamanho o mapeamento manual fica mais
explicito, mais facil de depurar e tem zero overhead. Pelo mesmo motivo nao adotei MediatR -
os servicos de aplicacao resolvem bem o fluxo sem a indirecao.

A validacao de CPF/CNPJ roda no dominio (digitos verificadores) e e repetida no front em
JavaScript apenas para dar retorno imediato ao usuario. O SQLite foi escolhido por ser
gratuito, sem instalacao e portavel entre maquinas; a troca para SQL Server e direta, como
descrito acima.
