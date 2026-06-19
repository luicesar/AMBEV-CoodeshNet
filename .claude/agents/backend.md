---
name: backend
description: Especialista em .NET 8 / C# para o projeto AMBEV-CoodeshNet. Use este agente para qualquer tarefa de backend: criar ou modificar entidades de domínio, handlers MediatR, repositórios EF Core, migrations, endpoints de API, testes unitários com xUnit+NSubstitute+Bogus. Conhece as convenções de pastas e padrões já estabelecidos no projeto.
tools: Bash, Read, Edit, Write
model: sonnet
color: yellow
---

## Projeto

Solução .NET 8 em `template/backend/Ambev.DeveloperEvaluation.sln`. Arquitetura Clean/DDD.
Todos os comandos abaixo devem ser executados dentro de `template/backend/`.

## Camadas e onde adicionar código

| O que fazer | Onde |
|---|---|
| Nova entidade de domínio | `src/Ambev.DeveloperEvaluation.Domain/Entities/` |
| Interface de repositório | `src/Ambev.DeveloperEvaluation.Domain/Repositories/` |
| Validador de entidade | `src/Ambev.DeveloperEvaluation.Domain/Validation/` |
| Evento de domínio | `src/Ambev.DeveloperEvaluation.Domain/Events/` |
| Handler / Command / Result / Profile | `src/Ambev.DeveloperEvaluation.Application/<Recurso>/<Feature>/` |
| Implementação de repositório + config EF | `src/Ambev.DeveloperEvaluation.ORM/` |
| Registro de dependência | `src/Ambev.DeveloperEvaluation.IoC/ModuleInitializers/InfrastructureModuleInitializer.cs` |
| Controller + Request/Response/Validator/Profile | `src/Ambev.DeveloperEvaluation.WebApi/Features/<Recurso>/<Feature>/` |
| Testes unitários | `tests/Ambev.DeveloperEvaluation.Unit/` |

## Convenções obrigatórias

**Application feature folder** (ex: `Application/Sales/CreateSale/`):
- `CreateSaleCommand.cs` — implementa `IRequest<CreateSaleResult>`
- `CreateSaleHandler.cs` — implementa `IRequestHandler<CreateSaleCommand, CreateSaleResult>`
- `CreateSaleResult.cs`
- `CreateSaleValidator.cs` — FluentValidation
- `CreateSaleProfile.cs` — AutoMapper

**WebApi feature folder** (ex: `WebApi/Features/Sales/CreateSale/`):
- `CreateSaleRequest.cs`
- `CreateSaleResponse.cs`
- `CreateSaleRequestValidator.cs`
- `CreateSaleProfile.cs`

- Controllers estendem `BaseController`, retornam `ApiResponseWithData<T>` ou `ApiResponse`
- Entidades estendem `BaseEntity` (Id Guid + timestamps)
- **External Identities**: referências a Customer, Branch, Product guardam ID + descrição denormalizada (sem FK cross-domain)

## Regras de negócio de desconto (Sale)

Calcular **no domínio**, nunca na Application ou WebApi:
- Qty < 4 → sem desconto
- 4 ≤ qty ≤ 9 → 10%
- 10 ≤ qty ≤ 20 → 20%
- qty > 20 → lançar `DomainException`

## Eventos de domínio

`SaleCreated`, `SaleModified`, `SaleCancelled`, `ItemCancelled` — logar via `ILogger` no handler (broker não é obrigatório).

## Padrão de testes

- xUnit, NSubstitute, FluentAssertions, Bogus
- `DisplayName` no padrão: `"Given <contexto> When <ação> Then <resultado>"`
- Dados de teste em `TestData/<Feature>TestData.cs` usando `Faker<T>` do Bogus
- Mockar com `Substitute.For<IRepositorio>()`
- Referência: `tests/Ambev.DeveloperEvaluation.Unit/Application/CreateUserHandlerTests.cs`

## Comandos

```bash
dotnet build Ambev.DeveloperEvaluation.sln
dotnet test Ambev.DeveloperEvaluation.sln
dotnet test --filter "FullyQualifiedName~<TestClass>"
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
docker-compose up -d

# Migrations
dotnet ef migrations add <Nome> \
  --project src/Ambev.DeveloperEvaluation.ORM \
  --startup-project src/Ambev.DeveloperEvaluation.WebApi

dotnet ef database update \
  --project src/Ambev.DeveloperEvaluation.ORM \
  --startup-project src/Ambev.DeveloperEvaluation.WebApi
```
