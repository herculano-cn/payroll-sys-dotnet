import axios from 'axios';
import type { Employee, CreateEmployeeDto, UpdateEmployeeDto, ApiResponse } from '../types/employee';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor for adding auth token (if needed in future)
api.interceptors.request.use(
  (config) => {
    // const token = localStorage.getItem('token');
    // if (token) {
    //   config.headers.Authorization = `Bearer ${token}`;
    // }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor for handling errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response) {
      // Server responded with error
      console.error('API Error:', error.response.data);
    } else if (error.request) {
      // Request made but no response
      console.error('Network Error:', error.request);
    } else {
      // Something else happened
      console.error('Error:', error.message);
    }
    return Promise.reject(error);
  }
);

export const employeeApi = {
  /**
   * Get all employees
   * Maps to: GET /api/employees
   */
  getAll: async (includeDeleted = false): Promise<Employee[]> => {
    const response = await api.get<ApiResponse<Employee[]>>('/api/employees', {
      params: { includeDeleted },
    });
    return response.data.data || [];
  },

  /**
   * Get employee by ID
   * Maps to: GET /api/employees/{id}
   * User Story 2: Search Employee
   */
  getById: async (id: number): Promise<Employee> => {
    const response = await api.get<ApiResponse<Employee>>(`/api/employees/${id}`);
    if (!response.data.data) {
      throw new Error('Employee not found');
    }
    return response.data.data;
  },

  /**
   * Get employee by employee ID (business key)
   * Maps to: GET /api/employees/by-employee-id/{employeeId}
   * User Story 2: Search Employee
   */
  getByEmployeeId: async (employeeId: string): Promise<Employee> => {
    const response = await api.get<ApiResponse<Employee>>(
      `/api/employees/by-employee-id/${employeeId}`
    );
    if (!response.data.data) {
      throw new Error('Employee not found');
    }
    return response.data.data;
  },

  /**
   * Get employees by period
   * Maps to: GET /api/employees/by-period?month={month}&year={year}
   */
  getByPeriod: async (month: number, year: number): Promise<Employee[]> => {
    const response = await api.get<ApiResponse<Employee[]>>('/api/employees/by-period', {
      params: { month, year },
    });
    return response.data.data || [];
  },

  /**
   * Create new employee
   * Maps to: POST /api/employees
   * User Story 1: Employee Registration
   */
  create: async (employee: CreateEmployeeDto): Promise<Employee> => {
    const response = await api.post<ApiResponse<Employee>>('/api/employees', employee);
    if (!response.data.data) {
      throw new Error('Failed to create employee');
    }
    return response.data.data;
  },

  /**
   * Update employee
   * Maps to: PUT /api/employees/{id}
   * User Story 3: Modify Employee
   */
  update: async (id: number, employee: UpdateEmployeeDto): Promise<Employee> => {
    const response = await api.put<ApiResponse<Employee>>(`/api/employees/${id}`, employee);
    if (!response.data.data) {
      throw new Error('Failed to update employee');
    }
    return response.data.data;
  },

  /**
   * Delete employee (soft delete)
   * Maps to: DELETE /api/employees/{id}
   * User Story 4: Delete Employee
   */
  delete: async (id: number): Promise<void> => {
    await api.delete(`/api/employees/${id}`);
  },
};

export default api;