# Payroll Management System

## 🎯 Project Overview

Full-stack payroll management system built with .NET 9 and React 18.

**Backend**: .NET 9 Web API + Entity Framework Core + SQLite  
**Frontend**: React 18 + TypeScript + Vite  
**Infrastructure**: Docker + Docker Compose

---

## 📊 Project Status

### ✅ Completed

#### Documentation
- ✅ [`README.md`](README.md) - Project overview
- ✅ [`DOCKER.md`](DOCKER.md) - Docker deployment guide
- ✅ [`GIT_SETUP.md`](GIT_SETUP.md) - Git repository setup

#### Backend (.NET 9)
- ✅ **Payroll.Domain** - Domain entities and interfaces
  - ✅ [`Employee`](backend/Payroll.Domain/Entities/Employee.cs) entity (30+ fields)
  - ✅ [`IEmployeeRepository`](backend/Payroll.Domain/Interfaces/IEmployeeRepository.cs) interface
  - ✅ Custom exceptions ([`NotFoundException`](backend/Payroll.Domain/Exceptions/NotFoundException.cs), [`ValidationException`](backend/Payroll.Domain/Exceptions/ValidationException.cs), [`DuplicateException`](backend/Payroll.Domain/Exceptions/DuplicateException.cs))

- ✅ **Payroll.Application** - Business logic layer
  - ✅ [`CNPJValidationService`](backend/Payroll.Application/Services/CNPJValidationService.cs) - Brazilian tax ID validation
  - ✅ [`TaxCalculationService`](backend/Payroll.Application/Services/TaxCalculationService.cs) - INSS & IRRF calculations
  - ✅ [`PayrollCalculationService`](backend/Payroll.Application/Services/PayrollCalculationService.cs) - Complete payroll calculations
  - ✅ [`EmployeeService`](backend/Payroll.Application/Services/EmployeeService.cs) - CRUD operations
  - ✅ [`EmployeeValidator`](backend/Payroll.Application/Validators/EmployeeValidator.cs) - FluentValidation rules

- ✅ **Payroll.Infrastructure** - Data access layer
  - ✅ [`PayrollDbContext`](backend/Payroll.Infrastructure/Data/PayrollDbContext.cs) - EF Core context with SQLite
  - ✅ [`EmployeeRepository`](backend/Payroll.Infrastructure/Repositories/EmployeeRepository.cs) - Repository implementation

- ✅ **Payroll.API** - Web API project
  - ✅ REST Controllers ([`EmployeesController`](backend/Payroll.API/Controllers/EmployeesController.cs))
  - ✅ DTOs (Data Transfer Objects)
  - ✅ Exception handling middleware
  - ✅ Swagger/OpenAPI configuration
  - ✅ Health check endpoint

#### Frontend (React + TypeScript)
- ✅ **React Application**
  - ✅ Employee registration form
  - ✅ Employee search interface
  - ✅ Employee list with edit/delete
  - ✅ Real-time validation
  - ✅ API client integration
  - ✅ Responsive design

#### Infrastructure
- ✅ **Docker Configuration**
  - ✅ Dockerfile for backend
  - ✅ Dockerfile for frontend
  - ✅ docker-compose.yml
  - ✅ Nginx configuration

### 📋 Pending

- ⏳ **Payroll.Tests** - Unit & integration tests
- ⏳ API documentation enhancements
- ⏳ Deployment guide

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────┐
│         React SPA (TypeScript)          │
│    Employee Management Interface        │
└─────────────────────────────────────────┘
                    │
            HTTPS + REST API
                    │
┌─────────────────────────────────────────┐
│      ASP.NET Core Web API (.NET 9)      │
│  - RESTful Endpoints                    │
│  - Request Validation                   │
│  - Swagger Documentation                │
└─────────────────────────────────────────┘
                    │
┌─────────────────────────────────────────┐
│         Business Logic Layer            │
│  - PayrollCalculationService            │
│  - TaxCalculationService                │
│  - CNPJValidationService                │
│  - EmployeeService                      │
└─────────────────────────────────────────┘
                    │
┌─────────────────────────────────────────┐
│         Data Access Layer               │
│  - Entity Framework Core                │
│  - Repository Pattern                   │
└─────────────────────────────────────────┘
                    │
┌─────────────────────────────────────────┐
│         SQLite Database                 │
│  - Employees Table                      │
│  - Audit Fields                         │
└─────────────────────────────────────────┘
```

---

## 🔑 Key Features

### Business Logic

1. **CNPJ Validation** - Brazilian tax ID validation
   - 14-digit validation
   - Weighted checksum calculation
   - Two verification digits

2. **Tax Calculations**
   - **INSS** (Social Security): Progressive rates (8%, 9%, 11%)
   - **IRRF** (Income Tax): Progressive rates (7.5%, 15%, 22.5%, 27.5%)
   - Dependent deductions (R$ 189.59 per dependent)

3. **Payroll Calculations**
   - Overtime pay (150% of hourly rate)
   - Weekly rest (DSR)
   - Gross salary
   - Transportation voucher (6% of base salary)
   - Family allowance (R$ 41.37 or R$ 29.16 per child)
   - Absence deduction
   - FGTS (8% of gross salary)
   - Net salary

4. **Validation Rules**
   - Reference month: 1-12
   - Reference year: >1959
   - Name/Position: Alphabetic only
   - CNPJ: Valid Brazilian format
   - Hire date: Valid and >1959
   - All numeric fields: Proper ranges

### Core Features

1. ✅ **Employee Registration** - Create new payroll records with full validation
2. ✅ **Employee Search** - Find employees by ID or employee number
3. ✅ **Employee Modification** - Update existing records with recalculation
4. ✅ **Employee Deletion** - Soft delete with audit trail

---

## 🚀 Quick Start

### Prerequisites
- .NET 9 SDK
- Node.js 20 LTS
- Docker (optional)

### Option 1: Docker (Recommended)

```bash
# Start all services
docker-compose up -d

# Access the application
# Frontend: http://localhost:3000
# Backend API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

### Option 2: Manual Setup

#### Backend
```bash
cd backend
dotnet restore
dotnet run --project Payroll.API
# API available at http://localhost:5000
```

#### Frontend
```bash
cd frontend
npm install
npm run dev
# App available at http://localhost:3000
```

---

## 📚 Documentation

- **[Docker Guide](DOCKER.md)** - Complete Docker deployment guide
- **[Git Setup](GIT_SETUP.md)** - Repository setup instructions
- **[Backend README](backend/README.md)** - Backend API documentation
- **[Frontend README](frontend/README.md)** - Frontend application guide
- **API Documentation** - Available at `/swagger` when API is running

---

## 🧪 Testing

### Run Unit Tests
```bash
cd backend
dotnet test Payroll.Tests
```

### Test Coverage Target
- Business Logic: >90%
- Controllers: >80%
- Overall: >80%

---

## 🔒 Security

- Input validation with FluentValidation
- SQL injection protection (EF Core parameterized queries)
- Soft delete for audit trail
- HTTPS enforced in production
- CORS configured

---

## 💾 Database

### SQLite (Development)
- Zero configuration
- File-based: `payroll.db`
- Perfect for development and small deployments

### Migration Path to SQL Server
```csharp
// Simply change connection string in appsettings.json
// From: "Data Source=payroll.db"
// To: "Server=localhost;Database=Payroll;..."
```

---

## 📊 Code Metrics

- **Backend Lines**: ~6,000+ lines
- **Frontend Lines**: ~2,000+ lines
- **Total Files**: ~50+ files
- **Test Coverage**: Target >80%

---

## 🎯 Business Rules

All critical business rules implemented:
1. Date validations (month, year, hire date)
2. CNPJ validation algorithm
3. Overtime calculation (150% rate)
4. Weekly rest (DSR) calculation
5. INSS progressive rates
6. IRRF progressive rates with deductions
7. Family allowance brackets
8. Transportation voucher (6%)
9. FGTS (8%)
10. Absence deduction

---

## 🛠️ Technology Stack

### Backend
- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- FluentValidation
- Swagger/OpenAPI

### Frontend
- React 18
- TypeScript
- Vite
- Axios
- React Router
- TanStack Query

### Infrastructure
- Docker
- Docker Compose
- Nginx

---

## 📝 License

[To be defined]

---

## 🔄 Next Steps

1. ✅ Complete API implementation
2. ✅ Build React frontend
3. ✅ Configure Docker
4. ⏳ Create comprehensive test suite
5. ⏳ Generate enhanced API documentation
6. ⏳ Create deployment guide
7. ⏳ Performance testing
8. ⏳ Security audit
9. ⏳ User acceptance testing
10. ⏳ Production deployment

---

**Last Updated**: 2024-12-12  
**Status**: ✅ Full-Stack Application Complete  
**Version**: 1.0.0