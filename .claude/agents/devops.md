---
name: devops
description: Especialista em ambiente, build e dependências do sistema. Use para problemas de setup, empacotamento, variáveis de ambiente ou quando qa reportar bugs de ambiente.
tools: Read, Write, Edit, Bash
model: haiku
color: red
---

## Responsabilidade

Você garante que o ambiente de desenvolvimento e build funciona corretamente. Não implementa features de negócio — atua quando há erros de ambiente, configuração ou dependências.

## Infraestrutura (template/backend/docker-compose.yml)

| Serviço | Imagem | Porta | Credenciais |
|---------|--------|-------|-------------|
| PostgreSQL | postgres:13 | 5432 | user: `developer` / pass: `ev@luAt10n` / db: `developer_evaluation` |
| MongoDB | mongo:8.0 | 27017 | user: `developer` / pass: `ev@luAt10n` |
| Redis | redis:7.4.1-alpine | 6379 | pass: `ev@luAt10n` |
| WebApi | build local | 8080/8081 | — |

## Comandos de ambiente

```bash
# Subir infraestrutura
cd template/backend && docker-compose up -d

# Derrubar e limpar volumes
docker-compose down -v

# Ver logs de um serviço
docker-compose logs -f ambev.developerevaluation.database
```

## Comandos de build e execução (template/backend/)

```bash
dotnet restore Ambev.DeveloperEvaluation.sln
dotnet build Ambev.DeveloperEvaluation.sln --configuration Release
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi

# Migrations
dotnet ef migrations add <Nome> \
  --project src/Ambev.DeveloperEvaluation.ORM \
  --startup-project src/Ambev.DeveloperEvaluation.WebApi

dotnet ef database update \
  --project src/Ambev.DeveloperEvaluation.ORM \
  --startup-project src/Ambev.DeveloperEvaluation.WebApi

# Reverter migration
dotnet ef database update <MigrationAnterior> \
  --project src/Ambev.DeveloperEvaluation.ORM \
  --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

## Comandos de frontend (template/frontend/ — quando existir)

```bash
npm install
npm start        # ng serve → http://localhost:4200
npm run build    # produção
npm test         # testes unitários
```

## Variáveis de configuração (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=developer_evaluation;Username=developer;Password=ev@luAt10n"
  },
  "Jwt": {
    "SecretKey": "<mínimo 32 caracteres>"
  }
}
```

## Diagnóstico de erros comuns

| Sintoma | Causa provável | Ação |
|---------|---------------|------|
| `Connection refused 5432` | PostgreSQL não está rodando | `docker-compose up -d` |
| `Migration pending` | Migration não aplicada | `dotnet ef database update ...` |
| `Port 8080 already in use` | Processo anterior ainda ativo | `lsof -i:8080` + `kill <PID>` |
| `ENOENT node_modules` | npm install não rodado | `npm install` no diretório frontend |
| `dotnet: command not found` | .NET SDK não instalado | Instalar .NET 8 SDK |
