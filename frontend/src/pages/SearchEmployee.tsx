import { useState } from 'react';
import { employeeApi } from '../services/api';
import type { Employee } from '../types/employee';

export default function SearchEmployee() {
  const [searchType, setSearchType] = useState<'id' | 'employeeId'>('employeeId');
  const [searchValue, setSearchValue] = useState('');
  const [employee, setEmployee] = useState<Employee | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setEmployee(null);

    try {
      let result: Employee;
      if (searchType === 'id') {
        result = await employeeApi.getById(Number(searchValue));
      } else {
        result = await employeeApi.getByEmployeeId(searchValue);
      }
      setEmployee(result);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Employee not found');
    } finally {
      setLoading(false);
    }
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('pt-BR');
  };

  return (
    <div className="search-container">
      <h1>Search Employee</h1>
      <p className="page-subtitle">Find employees by employee ID or system ID</p>

      <form onSubmit={handleSearch} className="search-form">
        <div className="search-type">
          <label>
            <input
              type="radio"
              value="employeeId"
              checked={searchType === 'employeeId'}
              onChange={(e) => setSearchType(e.target.value as 'employeeId')}
            />
            <span>Search by Employee ID</span>
          </label>
          <label>
            <input
              type="radio"
              value="id"
              checked={searchType === 'id'}
              onChange={(e) => setSearchType(e.target.value as 'id')}
            />
            <span>Search by System ID</span>
          </label>
        </div>

        <div className="search-input-group">
          <input
            type={searchType === 'id' ? 'number' : 'text'}
            value={searchValue}
            onChange={(e) => setSearchValue(e.target.value)}
            placeholder={searchType === 'id' ? 'Enter system ID' : 'Enter employee ID'}
            required
          />
          <button type="submit" disabled={loading} className="btn btn-primary">
            {loading ? 'Searching...' : '🔍 Search'}
          </button>
        </div>
      </form>

      {error && (
        <div className="alert alert-error">
          {error}
        </div>
      )}

      {employee && (
        <div className="employee-details">
          <div className="details-header">
            <h2>{employee.name}</h2>
            <span className="employee-badge">Employee ID: {employee.employeeId}</span>
          </div>

          <div className="details-grid">
            <div className="detail-section">
              <h3>Personal Information</h3>
              <div className="detail-item">
                <span className="label">Position:</span>
                <span className="value">{employee.position}</span>
              </div>
              <div className="detail-item">
                <span className="label">CNPJ:</span>
                <span className="value">{employee.cnpj}</span>
              </div>
              <div className="detail-item">
                <span className="label">Hire Date:</span>
                <span className="value">{formatDate(employee.hireDate)}</span>
              </div>
              <div className="detail-item">
                <span className="label">Period:</span>
                <span className="value">
                  {employee.referenceMonth.toString().padStart(2, '0')}/{employee.referenceYear}
                </span>
              </div>
            </div>

            <div className="detail-section">
              <h3>Work Information</h3>
              <div className="detail-item">
                <span className="label">Working Hours:</span>
                <span className="value">{employee.workingHours}h</span>
              </div>
              <div className="detail-item">
                <span className="label">Overtime Hours:</span>
                <span className="value">{employee.overtimeHours}h</span>
              </div>
              <div className="detail-item">
                <span className="label">Absences:</span>
                <span className="value">{employee.absences} days</span>
              </div>
              <div className="detail-item">
                <span className="label">Dependents:</span>
                <span className="value">{employee.dependents}</span>
              </div>
              <div className="detail-item">
                <span className="label">Children:</span>
                <span className="value">{employee.children}</span>
              </div>
            </div>

            <div className="detail-section highlight">
              <h3>💰 Earnings</h3>
              <div className="detail-item">
                <span className="label">Base Salary:</span>
                <span className="value">{formatCurrency(employee.baseSalary)}</span>
              </div>
              <div className="detail-item">
                <span className="label">Overtime:</span>
                <span className="value">{formatCurrency(employee.totalOvertime)}</span>
              </div>
              <div className="detail-item">
                <span className="label">Weekly Rest (DSR):</span>
                <span className="value">{formatCurrency(employee.weeklyRest)}</span>
              </div>
              <div className="detail-item total">
                <span className="label">Gross Salary:</span>
                <span className="value">{formatCurrency(employee.grossSalary)}</span>
              </div>
            </div>

            <div className="detail-section highlight">
              <h3>📉 Deductions</h3>
              <div className="detail-item">
                <span className="label">Social Security (INSS):</span>
                <span className="value negative">{formatCurrency(employee.inss)}</span>
              </div>
              <div className="detail-item">
                <span className="label">Income Tax (IRRF):</span>
                <span className="value negative">{formatCurrency(employee.irrf)}</span>
              </div>
              <div className="detail-item">
                <span className="label">Transportation Voucher:</span>
                <span className="value negative">{formatCurrency(employee.transportationVoucher)}</span>
              </div>
              <div className="detail-item">
                <span className="label">Absences:</span>
                <span className="value negative">{formatCurrency(employee.absenceDeduction)}</span>
              </div>
            </div>

            <div className="detail-section highlight">
              <h3>🎁 Benefits</h3>
              <div className="detail-item">
                <span className="label">Family Allowance:</span>
                <span className="value">{formatCurrency(employee.familyAllowance)}</span>
              </div>
              <div className="detail-item">
                <span className="label">FGTS (8%):</span>
                <span className="value">{formatCurrency(employee.fgts)}</span>
              </div>
            </div>

            <div className="detail-section highlight total-section">
              <h3>💵 Net Salary</h3>
              <div className="detail-item total">
                <span className="label">Total to Receive:</span>
                <span className="value large">{formatCurrency(employee.netSalary)}</span>
              </div>
            </div>
          </div>

          <div className="details-footer">
            <p className="text-muted">
              Created at: {formatDate(employee.createdAt)} | 
              Last updated: {formatDate(employee.updatedAt)}
            </p>
          </div>
        </div>
      )}
    </div>
  );
}