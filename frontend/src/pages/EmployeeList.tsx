import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { employeeApi } from '../services/api';
import type { Employee } from '../types/employee';

export default function EmployeeList() {
  const queryClient = useQueryClient();
  const [deleteId, setDeleteId] = useState<number | null>(null);

  const { data: employees = [], isLoading, error } = useQuery({
    queryKey: ['employees'],
    queryFn: () => employeeApi.getAll(),
  });

  const deleteMutation = useMutation({
    mutationFn: employeeApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['employees'] });
      setDeleteId(null);
    },
  });

  const handleDelete = (id: number) => {
    if (window.confirm('Are you sure you want to delete this employee?')) {
      deleteMutation.mutate(id);
    }
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value);
  };

  if (isLoading) {
    return (
      <div className="loading-container">
        <div className="spinner"></div>
        <p>Loading employees...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="error-container">
        <h2>Error loading employees</h2>
        <p>{(error as Error).message}</p>
      </div>
    );
  }

  return (
    <div className="list-container">
      <div className="list-header">
        <div>
          <h1>Employee List</h1>
          <p className="page-subtitle">View, edit, and manage all employees</p>
        </div>
        <Link to="/create" className="btn btn-primary">
          ➕ New Employee
        </Link>
      </div>

      {employees.length === 0 ? (
        <div className="empty-state">
          <div className="empty-icon">📋</div>
          <h2>No employees registered</h2>
          <p>Start by registering your first employee</p>
          <Link to="/create" className="btn btn-primary">
            Create Employee
          </Link>
        </div>
      ) : (
        <>
          <div className="list-stats">
            <div className="stat-item">
              <span className="stat-value">{employees.length}</span>
              <span className="stat-label">Employees</span>
            </div>
            <div className="stat-item">
              <span className="stat-value">
                {formatCurrency(
                  employees.reduce((sum, emp) => sum + emp.grossSalary, 0)
                )}
              </span>
              <span className="stat-label">Total Gross Payroll</span>
            </div>
            <div className="stat-item">
              <span className="stat-value">
                {formatCurrency(
                  employees.reduce((sum, emp) => sum + emp.netSalary, 0)
                )}
              </span>
              <span className="stat-label">Total Net Payroll</span>
            </div>
          </div>

          <div className="table-container">
            <table className="employee-table">
              <thead>
                <tr>
                  <th>Employee ID</th>
                  <th>Name</th>
                  <th>Position</th>
                  <th>Period</th>
                  <th>Base Salary</th>
                  <th>Gross Salary</th>
                  <th>Net Salary</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {employees.map((employee: Employee) => (
                  <tr key={employee.id}>
                    <td>
                      <span className="employee-id">{employee.employeeId}</span>
                    </td>
                    <td>
                      <div className="employee-name">
                        <strong>{employee.name}</strong>
                      </div>
                    </td>
                    <td>{employee.position}</td>
                    <td>
                      {employee.referenceMonth.toString().padStart(2, '0')}/
                      {employee.referenceYear}
                    </td>
                    <td>{formatCurrency(employee.baseSalary)}</td>
                    <td className="highlight">{formatCurrency(employee.grossSalary)}</td>
                    <td className="highlight success">
                      {formatCurrency(employee.netSalary)}
                    </td>
                    <td>
                      <div className="action-buttons">
                        <Link
                          to={`/edit/${employee.id}`}
                          className="btn btn-sm btn-secondary"
                          title="Edit"
                        >
                          ✏️
                        </Link>
                        <button
                          onClick={() => handleDelete(employee.id)}
                          className="btn btn-sm btn-danger"
                          title="Delete"
                          disabled={deleteMutation.isPending && deleteId === employee.id}
                        >
                          {deleteMutation.isPending && deleteId === employee.id ? '⏳' : '🗑️'}
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </>
      )}
    </div>
  );
}