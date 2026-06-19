# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Goal

This is a developer evaluation project (Coodesh / Ambev) requiring implementation of a **Sales API** with full CRUD. All backend code lives under `template/backend/`. The core task is building the Sale entity with quantity-based discount rules:
- 4–9 identical items: 10% discount
- 10–20 identical items: 20% discount
- Max 20 identical items per product; no discount below 4 items

Optional: log domain events (`SaleCreated`, `SaleModified`, `SaleCancelled`, `ItemCancelled`) — no broker required.

## Commands

All commands run from `template/backend/`.

```bash
# Build
dotnet build Ambev.DeveloperEvaluation.sln

# Run API (requires running infrastructure)
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi

# Run all tests
dotnet test Ambev.DeveloperEvaluation.sln

# Run a single test class
dotnet test --filter "FullyQualifiedName~CreateUserHandlerTests"

# Run with coverage report
./coverage-report.sh

# Start infrastructure (PostgreSQL, MongoDB, Redis)
docker-compose up -d

# Add EF migration
dotnet ef migrations add <MigrationName> \
  --project src/Ambev.DeveloperEvaluation.ORM \
  --startup-project src/Ambev.DeveloperEvaluation.WebApi

# Apply migrations
dotnet ef database update \
  --project src/Ambev.DeveloperEvaluation.ORM \
  --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

Infrastructure defaults (from docker-compose): PostgreSQL on 5432, MongoDB on 27017, Redis on 6379. Credentials: user `developer`, password `ev@luAt10n`.

## Architecture

Clean Architecture with DDD. Dependency direction: `WebApi → Application → Domain ← ORM`. `Common` and `IoC` are cross-cutting.

```
src/
  Ambev.DeveloperEvaluation.Domain      # Entities, value objects, repo interfaces, specs, validators
  Ambev.DeveloperEvaluation.Application # MediatR handlers, commands/queries, AutoMapper profiles
  Ambev.DeveloperEvaluation.ORM         # EF Core DbContext, repo implementations, migrations
  Ambev.DeveloperEvaluation.Common      # JWT, BCrypt, FluentValidation pipeline, Serilog, health checks
  Ambev.DeveloperEvaluation.IoC         # Module initializers (Application, Infrastructure, WebApi)
  Ambev.DeveloperEvaluation.WebApi      # Controllers, request/response DTOs, AutoMapper profiles, middleware
tests/
  Ambev.DeveloperEvaluation.Unit        # xUnit + NSubstitute + Bogus
  Ambev.DeveloperEvaluation.Integration
  Ambev.DeveloperEvaluation.Functional
```

## Key Patterns

**CQRS via MediatR**: Every operation is a `Command` or `Query` with a corresponding `Handler`. Validation runs twice — once in the controller via a `*RequestValidator` (FluentValidation), and once inside the handler via a `*CommandValidator`. A MediatR `ValidationBehavior<TRequest,TResponse>` pipeline also runs registered validators automatically.

**Feature folders in WebApi**: Each feature lives in `Features/<Resource>/<FeatureName>/` and contains `*Request`, `*Response`, `*RequestValidator`, and `*Profile` (AutoMapper). Controllers dispatch to MediatR and map results.

**Application layer feature folders**: `Application/<Resource>/<FeatureName>/` contains `*Command`, `*Handler`, `*Result`, `*Validator`, and `*Profile`.

**Repository pattern**: Domain defines interfaces (`IUserRepository`); ORM implements them. EF Core (`DefaultContext`) uses `ApplyConfigurationsFromAssembly` — add entity configs in `ORM/Mapping/`.

**External Identities (DDD)**: When referencing entities from another domain (e.g., Customer, Branch, Product on a Sale), store the ID plus a denormalized description snapshot — do not create hard foreign keys across domain boundaries.

**Domain events**: Raise events on domain entities (e.g., `UserRegisteredEvent`). The evaluation expects sale events to be at minimum logged.

## Adding a New Feature (e.g., Sale)

1. **Domain** — add entity under `Domain/Entities/`, repository interface under `Domain/Repositories/`, FluentValidation validator under `Domain/Validation/`.
2. **Application** — add folder `Application/Sales/<FeatureName>/` with `Command`, `Handler`, `Result`, `Validator`, `Profile`.
3. **ORM** — add EF mapping under `ORM/Mapping/`, register `DbSet` in `DefaultContext`, create migration.
4. **IoC** — register the repository in `InfrastructureModuleInitializer`.
5. **WebApi** — add controller under `Features/Sales/`, add request/response/validator/profile per operation.
6. **Tests** — mirror the Application handler test pattern in `Unit/Application/` using NSubstitute mocks and Bogus test data factories.
