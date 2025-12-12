/**
 * Employee types matching the backend DTOs
 */

export interface Employee {
  id: number;
  referenceMonth: number;
  referenceYear: number;
  employeeId: string;
  name: string;
  position: string;
  cnpj: string;
  hireDate: string;
  absences: number;
  baseSalary: number;
  workingHours: number;
  overtimeHours: number;
  dependents: number;
  children: number;
  optInTransportationVoucher: boolean;
  
  // Calculated fields
  grossSalary: number;
  netSalary: number;
  totalOvertime: number;
  weeklyRest: number;
  inss: number;
  irrf: number;
  familyAllowance: number;
  dependentDeduction: number;
  transportationVoucher: number;
  absenceDeduction: number;
  fgts: number;
  
  // Audit fields
  createdAt: string;
  updatedAt: string;
}

export interface CreateEmployeeDto {
  referenceMonth: number;
  referenceYear: number;
  employeeId: string;
  name: string;
  position: string;
  cnpj: string;
  hireDate: string;
  absences: number;
  baseSalary: number;
  workingHours: number;
  overtimeHours: number;
  dependents: number;
  children: number;
  optInTransportationVoucher: boolean;
}

export interface UpdateEmployeeDto extends CreateEmployeeDto {
  id: number;
}

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: Record<string, string[]>;
}