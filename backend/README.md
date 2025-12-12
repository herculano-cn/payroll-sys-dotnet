# Payroll API - Backend

RESTful API for payroll management built with .NET 9.

## 🚀 Quick Start

### Prerequisites
- .NET 9 SDK ([Download](https://dotnet.microsoft.com/download/dotnet/9.0))

### Run the API

```bash
# Restore dependencies
dotnet restore

# Run the API
dotnet run --project Payroll.API

# The API will be available at:
# - HTTP: http://localhost:5000
# - HTTPS: https://localhost:5001
# - Swagger: http://localhost:5000/swagger
```

### Create/Update Database

```bash
# SQLite database is created automatically on first run
# File: payroll.db (in the API project root)

# To recreate the database (if needed):
dotnet ef database drop --project Payroll.Infrastructure --startup-project Payroll.API
dotnet ef database update --project Payroll.Infrastructure --startup-project Payroll.API
```

## 📚 API Documentation

Access interactive Swagger documentation at: **http://localhost:5000/swagger**

### Main Endpoints

#### Employees

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/employees` | Create new employee |
| GET | `/api/employees/{id}` | Get by ID |
| GET | `/api/employees/by-employee-id/{employeeId}` | Get by employee number |
| GET | `/api/employees` | List all |
| GET | `/api/employees/by-period?month=X&year=Y` | Get by period |
| PUT | `/api/employees/{id}` | Update employee |
| DELETE | `/api/employees/{id}` | Delete employee (soft delete) |

### Request Example

**POST /api/employees**

```json
{
  "referenceMonth": 12,
  "referenceYear": 2024,
  "employeeId": "12345",
  "name": "John Doe",
  "position": "Developer",
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

**Response (201 Created)**

```json
{
  "success": true,
  "data": {
    "id": 1,
    "referenceMonth": 12,
    "referenceYear": 2024,
    "employeeId": "12345",
    "name": "John Doe",
    "position": "Developer",
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

## 🏗️ Architecture

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

## 🧪 Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## 📊 Business Rules Implemented

### Validations
- ✅ Reference month: 1-12
- ✅ Reference year: >1959
- ✅ CNPJ: Complete validation algorithm (14 digits + checksum)
- ✅ Name/Position: Letters only
- ✅ Hire date: Valid and >1959

### Calculations
- ✅ **Overtime**: 150% of hourly rate
- ✅ **Weekly Rest (DSR)**: (Overtime / 26) * 4
- ✅ **INSS**: Progressive rates (8%, 9%, 11%)
- ✅ **IRRF**: Progressive rates (7.5%, 15%, 22.5%, 27.5%)
- ✅ **Family Allowance**: R$ 41.37 or R$ 29.16 per child
- ✅ **Transportation Voucher**: 6% of base salary (optional)
- ✅ **FGTS**: 8% of gross salary
- ✅ **Absence Deduction**: (Base salary / 30) * Absences

## 🔒 Security

- ✅ Input validation with FluentValidation
- ✅ SQL Injection protection (EF Core parameterized queries)
- ✅ Soft delete for audit trail
- ✅ CORS configured
- ✅ HTTPS enforced in production

## 📝 Logs

Logs are written to console and can be configured in `appsettings.json`:

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

### Error: "Database is locked"
- Close all connections to SQLite database
- Delete `payroll.db` file and run again

### Error: "Port already in use"
- Change port in `Properties/launchSettings.json`
- Or kill the process: `lsof -ti:5000 | xargs kill`

### Error: "Package restore failed"
- Run: `dotnet restore --force`
- Clear cache: `dotnet nuget locals all --clear`

## 📦 Production Build

```bash
# Build Release
dotnet build -c Release

# Publish
dotnet publish -c Release -o ./publish

# Run published app
cd publish
dotnet Payroll.API.dll
```

## 🔄 Migration to SQL Server

To migrate from SQLite to SQL Server, change the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Payroll;User Id=sa;Password=YourPassword;TrustServerCertificate=true"
  }
}
```

Install the package:
```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

Update in `Program.cs`:
```csharp
// From:
options.UseSqlite(connectionString)

// To:
options.UseSqlServer(connectionString)
```

## 📚 Additional Resources

- [.NET 9 Documentation](https://docs.microsoft.com/dotnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- [FluentValidation](https://docs.fluentvalidation.net/)

## 🤝 Contributing

1. Fork the project
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

[To be defined]

---

**Last Updated**: 2024-12-12  
**Version**: 1.0.0  
**Status**: ✅ API Complete and Functional