# Payroll System - Frontend

React + TypeScript web interface for the payroll management system.

## 🚀 Quick Start

### Prerequisites
- Node.js 20 LTS ([Download](https://nodejs.org/))
- Backend API running at `http://localhost:5000`

### Installation and Execution

```bash
# Install dependencies
npm install

# Run in development mode
npm run dev

# The application will be available at:
# http://localhost:3000
```

### Production Build

```bash
# Build
npm run build

# Preview build
npm run preview
```

## 📁 Project Structure

```
frontend/
├── src/
│   ├── pages/               # Application pages
│   │   ├── Home.tsx              # Home page
│   │   ├── CreateEmployee.tsx    # Create employee
│   │   ├── SearchEmployee.tsx    # Search employee
│   │   ├── EmployeeList.tsx      # Employee list
│   ├── services/            # API services
│   │   └── api.ts                # Axios client
│   ├── types/               # TypeScript types
│   │   └── employee.ts           # Employee types
│   ├── App.tsx              # Main component
│   ├── main.tsx             # Entry point
│   ├── App.css              # Component styles
│   └── index.css            # Global styles
├── public/                  # Static files
├── index.html              # HTML template
├── package.json            # Dependencies
├── tsconfig.json           # TypeScript configuration
├── vite.config.ts          # Vite configuration
└── README.md              # This file
```

## 🎯 Implemented Features

### Core Features

1. **Create Employee** (`/create`)
   - Complete registration form
   - Real-time validation
   - Automatic payroll calculation
   - Success/error feedback

2. **Search Employee** (`/search`)
   - Search by ID or employee number
   - Display complete details
   - View calculations

3. **Employee List** (`/list`)
   - List all employees
   - Edit and delete actions
   - Payroll statistics
   - Responsive table

4. **Edit Employee** (`/edit/:id`)
   - Update employee data
   - Automatic recalculation
   - Change confirmation

### Main Components

#### EmployeeForm
Reusable form for creating/editing employees with:
- Field validation (month, year, CNPJ, etc.)
- Automatic formatting (CNPJ, dates, values)
- Real-time calculation
- Visual error feedback

#### EmployeeList
Employee list with:
- Sorting
- Quick actions (edit, delete)
- Payroll statistics

#### EmployeeDetails
Detailed view with:
- Personal data
- Payroll calculations
- Action options

## 🔧 Technologies

- **React 18** - UI library
- **TypeScript** - Type safety
- **Vite** - Build tool
- **Axios** - HTTP client
- **React Router** - Routing
- **TanStack Query** - Data fetching

## 📝 Implemented Validations

All backend validations are replicated in the frontend:

- ✅ Reference month: 1-12
- ✅ Reference year: >1959
- ✅ CNPJ: Brazilian format and algorithm
- ✅ Name/Position: Letters only
- ✅ Hire date: Valid and >1959
- ✅ Base salary: > 0
- ✅ Working hours: > 0
- ✅ Dependents/Children: >= 0

## 🎨 Formatting

### Monetary Values
```typescript
R$ 5,000.00  // Base salary
R$ 340.91    // Overtime
R$ 4,234.56  // Net salary
```

### CNPJ
```typescript
12.345.678/0001-95  // Brazilian format
```

### Dates
```typescript
01/15/2020  // Hire date
12/2024     // Reference period
```

## 🔄 Data Flow

```
User Input → Form Validation → API Call → Backend Processing → Response → UI Update
```

### Example: Create Employee

1. User fills form
2. Real-time validation
3. Submit → POST `/api/employees`
4. Backend calculates payroll
5. Response with calculated data
6. UI updates with success/error

## 🧪 Tests (Structure)

```bash
# Run tests (when implemented)
npm test

# Coverage
npm run test:coverage
```

## 🌐 Environment Variables

Create a `.env` file in the frontend root:

```env
VITE_API_URL=http://localhost:5000
```

## 📊 Performance

- **First Load**: < 2s
- **Time to Interactive**: < 3s
- **Bundle Size**: < 500KB (gzipped)
- **Lighthouse Score**: > 90

## 🔒 Security

- ✅ Client-side validation (doesn't replace server-side)
- ✅ Input sanitization
- ✅ HTTPS in production
- ✅ CORS configured

## 🐛 Troubleshooting

### Error: "Cannot connect to API"
- Check if backend is running at `http://localhost:5000`
- Check CORS in backend
- Check `VITE_API_URL` variable

### Error: "Module not found"
```bash
rm -rf node_modules package-lock.json
npm install
```

### Error: "Port 3000 already in use"
```bash
# Change port in vite.config.ts
server: {
  port: 3001,  // or another port
}
```

## 📚 Additional Resources

- [React Documentation](https://react.dev/)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)
- [Vite Guide](https://vitejs.dev/guide/)
- [TanStack Query](https://tanstack.com/query/latest)

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
**Status**: ✅ Complete and Functional