# Payroll API - Backend

API RESTful para gerenciamento de folha de pagamento, modernizada de um sistema legado COBOL.

## 🚀 Quick Start

### Pré-requisitos
- .NET 8 SDK ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))

### Executar a API

```bash
# Restaurar dependências
dotnet restore

# Executar a API
dotnet run --project Payroll.API

# A API estará disponível em:
# - HTTP: http://localhost:5000
# - HTTPS: https://localhost:5001
# - Swagger: http://localhost:5000/swagger
```

### Criar/Atualizar Database

```bash
# O banco de dados SQLite é criado automaticamente na primeira execução
# Arquivo: payroll.db (na raiz do projeto API)

# Para recriar o banco (se necessário):
dotnet ef database drop --project Payroll.Infrastructure --startup-project Payroll.API
dotnet ef database update --project Payroll.Infrastructure --startup-project Payroll.API
```

## 📚 Documentação da API

Acesse a documentação interativa Swagger em: **http://localhost:5000/swagger**

### Endpoints Principais

#### Employees (Funcionários)

| Método | Endpoint | Descrição | User Story |
|--------|----------|-----------|------------|
| POST | `/api/employees` | Criar novo funcionário | US1: Employee Registration |
| GET | `/api/employees/{id}` | Buscar por ID | US2: Search Employee |
| GET | `/api/employees/by-employee-id/{employeeId}` | Buscar por matrícula | US2: Search Employee |
| GET | `/api/employees` | Listar todos | - |
| GET | `/api/employees/by-period?month=X&year=Y` | Buscar por período | - |
| PUT | `/api/employees/{id}` | Atualizar funcionário | US3: Modify Employee |
| DELETE | `/api/employees/{id}` | Deletar funcionário (soft delete) | US4: Delete Employee |

### Exemplo de Requisição

**POST /api/employees**

```json
{
  "referenceMonth": 12,
  "referenceYear": 2024,
  "employeeId": "12345",
  "name": "João Silva",
  "position": "Desenvolvedor",
  "cnpj": "12345678000195",
  "hireDate": "2020-01-15",
  "absences": 0,
  "baseSalary": 5000.00,
  "workingHours": 220,
  "overtimeHours": 10,
  "dependents": 2,
  "children": 1,
  "optInTransportationVoucher": true
}
```

**Resposta (201 Created)**

```json
{
  "success": true,
  "data": {
    "id": 1,
    "referenceMonth": 12,
    "referenceYear": 2024,
    "employeeId": "12345",
    "name": "João Silva",
    "position": "Desenvolvedor",
    "cnpj": "12345678000195",
    "hireDate": "2020-01-15T00:00:00",
    "absences": 0,
    "baseSalary": 5000.00,
    "workingHours": 220,
    "overtimeHours": 10,
    "dependents": 2,
    "children": 1,
    "optInTransportationVoucher": true,
    "grossSalary": 5340.91,
    "netSalary": 4234.56,
    "totalOvertime": 340.91,
    "weeklyRest": 52.45,
    "inss": 570.88,
    "irrf": 235.47,
    "familyAllowance": 0.00,
    "dependentDeduction": 379.18,
    "transportationVoucher": 300.00,
    "absenceDeduction": 0.00,
    "fgts": 427.27,
    "createdAt": "2024-12-12T14:00:00Z",
    "updatedAt": "2024-12-12T14:00:00Z"
  },
  "message": "Employee created successfully"
}
```

## 🏗️ Arquitetura

```
Payroll.API/              # API Layer (Controllers, DTOs, Middleware)
├── Controllers/          # REST Controllers
├── DTOs/                 # Data Transfer Objects
├── Middleware/           # Exception handling, etc.
└── Program.cs            # Startup configuration

Payroll.Application/      # Business Logic Layer
├── Services/             # Business services
│   ├── EmployeeService.cs
│   ├── PayrollCalculationService.cs
│   ├── TaxCalculationService.cs
│   └── CNPJValidationService.cs
└── Validators/           # FluentValidation validators

Payroll.Domain/           # Domain Layer
├── Entities/             # Domain entities
├── Interfaces/           # Repository interfaces
└── Exceptions/           # Custom exceptions

Payroll.Infrastructure/   # Data Access Layer
├── Data/                 # DbContext
└── Repositories/         # Repository implementations
```

## 🧪 Testes

```bash
# Executar todos os testes
dotnet test

# Executar com cobertura
dotnet test /p:CollectCoverage=true
```

## 📊 Regras de Negócio Implementadas

### Validações
- ✅ Mês de referência: 1-12
- ✅ Ano de referência: >1959
- ✅ CNPJ: Algoritmo completo de validação (14 dígitos + checksum)
- ✅ Nome/Cargo: Apenas letras
- ✅ Data de admissão: Válida e >1959

### Cálculos
- ✅ **Horas Extras**: 150% da taxa horária
- ✅ **Descanso Semanal (DSR)**: (Horas extras / 26) * 4
- ✅ **INSS**: Taxas progressivas (8%, 9%, 11%)
- ✅ **IRRF**: Taxas progressivas (7.5%, 15%, 22.5%, 27.5%)
- ✅ **Salário Família**: R$ 41,37 ou R$ 29,16 por filho
- ✅ **Vale Transporte**: 6% do salário base (opcional)
- ✅ **FGTS**: 8% do salário bruto
- ✅ **Desconto de Faltas**: (Salário base / 30) * Faltas

## 🔒 Segurança

- ✅ Validação de entrada com FluentValidation
- ✅ Proteção contra SQL Injection (EF Core parametrizado)
- ✅ Soft delete para auditoria
- ✅ CORS configurado
- ✅ HTTPS enforced em produção
- ⏳ JWT Authentication (preparado, não implementado ainda)

## 📝 Logs

Os logs são gravados no console e podem ser configurados em `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 🐛 Troubleshooting

### Erro: "Database is locked"
- Feche todas as conexões ao banco SQLite
- Delete o arquivo `payroll.db` e execute novamente

### Erro: "Port already in use"
- Altere a porta em `Properties/launchSettings.json`
- Ou mate o processo usando a porta: `lsof -ti:5000 | xargs kill`

### Erro: "Package restore failed"
- Execute: `dotnet restore --force`
- Limpe o cache: `dotnet nuget locals all --clear`

## 📦 Build para Produção

```bash
# Build Release
dotnet build -c Release

# Publicar
dotnet publish -c Release -o ./publish

# Executar publicação
cd publish
dotnet Payroll.API.dll
```

## 🔄 Migração para SQL Server

Para migrar de SQLite para SQL Server, altere apenas a connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Payroll;User Id=sa;Password=YourPassword;TrustServerCertificate=true"
  }
}
```

E instale o pacote:
```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

Altere em `Program.cs`:
```csharp
// De:
options.UseSqlite(connectionString)

// Para:
options.UseSqlServer(connectionString)
```

## 📚 Recursos Adicionais

- [Documentação .NET 8](https://docs.microsoft.com/dotnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- [FluentValidation](https://docs.fluentvalidation.net/)

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

[A definir]

---

**Última Atualização**: 2024-12-12  
**Versão**: 1.0.0  
**Status**: ✅ API Completa e Funcional