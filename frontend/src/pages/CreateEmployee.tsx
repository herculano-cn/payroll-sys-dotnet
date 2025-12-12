import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useMutation, useQuery } from '@tanstack/react-query';
import { employeeApi } from '../services/api';
import type { CreateEmployeeDto, Employee } from '../types/employee';

export default function CreateEmployee() {
  const navigate = useNavigate();
  const { id } = useParams();
  const isEdit = Boolean(id);

  const [formData, setFormData] = useState<CreateEmployeeDto>({
    referenceMonth: new Date().getMonth() + 1,
    referenceYear: new Date().getFullYear(),
    employeeId: '',
    name: '',
    position: '',
    cnpj: '',
    hireDate: '',
    absences: 0,
    baseSalary: 0,
    workingHours: 220,
    overtimeHours: 0,
    dependents: 0,
    children: 0,
    optInTransportationVoucher: false,
  });

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [successMessage, setSuccessMessage] = useState('');

  // Load employee data if editing
  const { data: employeeData } = useQuery<Employee>({
    queryKey: ['employee', id],
    queryFn: () => employeeApi.getById(Number(id)),
    enabled: isEdit,
  });

  useEffect(() => {
    if (employeeData) {
      setFormData({
        referenceMonth: employeeData.referenceMonth,
        referenceYear: employeeData.referenceYear,
        employeeId: employeeData.employeeId,
        name: employeeData.name,
        position: employeeData.position,
        cnpj: employeeData.cnpj,
        hireDate: employeeData.hireDate.split('T')[0],
        absences: employeeData.absences,
        baseSalary: employeeData.baseSalary,
        workingHours: employeeData.workingHours,
        overtimeHours: employeeData.overtimeHours,
        dependents: employeeData.dependents,
        children: employeeData.children,
        optInTransportationVoucher: employeeData.optInTransportationVoucher,
      });
    }
  }, [employeeData]);

  const createMutation = useMutation({
    mutationFn: employeeApi.create,
  });

  const updateMutation = useMutation({
    mutationFn: (data: any) => employeeApi.update(Number(id), data),
  });

  // Handle mutation success/error
  useEffect(() => {
    if (createMutation.isSuccess) {
      setSuccessMessage('Employee created successfully!');
      setTimeout(() => navigate('/list'), 2000);
    }
    if (createMutation.isError) {
      const error: any = createMutation.error;
      const apiErrors = error.response?.data?.errors || {};
      setErrors(apiErrors);
    }
  }, [createMutation.isSuccess, createMutation.isError, createMutation.error, navigate]);

  useEffect(() => {
    if (updateMutation.isSuccess) {
      setSuccessMessage('Employee updated successfully!');
      setTimeout(() => navigate('/list'), 2000);
    }
    if (updateMutation.isError) {
      const error: any = updateMutation.error;
      const apiErrors = error.response?.data?.errors || {};
      setErrors(apiErrors);
    }
  }, [updateMutation.isSuccess, updateMutation.isError, updateMutation.error, navigate]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setErrors({});
    setSuccessMessage('');

    // Frontend validation
    const newErrors: Record<string, string> = {};
    
    if (!formData.hireDate || formData.hireDate === '') {
      newErrors.hireDate = 'Hire date is required';
    }
    
    if (!formData.name || formData.name.trim() === '') {
      newErrors.name = 'Name is required';
    }
    
    if (!formData.employeeId || formData.employeeId.trim() === '') {
      newErrors.employeeId = 'Employee ID is required';
    }
    
    if (!formData.position || formData.position.trim() === '') {
      newErrors.position = 'Position is required';
    }
    
    if (!formData.cnpj || formData.cnpj.trim() === '') {
      newErrors.cnpj = 'CNPJ is required';
    }
    
    if (formData.baseSalary <= 0) {
      newErrors.baseSalary = 'Base salary must be greater than zero';
    }
    
    if (formData.workingHours <= 0) {
      newErrors.workingHours = 'Working hours must be greater than zero';
    }
    
    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      return;
    }

    if (isEdit) {
      updateMutation.mutate({ id: Number(id), ...formData });
    } else {
      createMutation.mutate(formData);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : 
              type === 'number' ? Number(value) : value,
    }));
  };

  return (
    <div className="form-container">
      <h1>{isEdit ? 'Edit Employee' : 'Create Employee'}</h1>
      <p className="form-subtitle">
        {isEdit ? 'Update employee data and recalculate payroll' : 'Fill in the new employee data for automatic payroll calculation'}
      </p>

      {successMessage && (
        <div className="alert alert-success">{successMessage}</div>
      )}

      {Object.keys(errors).length > 0 && (
        <div className="alert alert-error">
          <strong>Validation error:</strong>
          <ul>
            {Object.entries(errors).map(([field, messages]) => (
              <li key={field}>
                {Array.isArray(messages) ? messages.join(', ') : messages}
              </li>
            ))}
          </ul>
        </div>
      )}

      <form onSubmit={handleSubmit} className="employee-form">
        <div className="form-section">
          <h2>Reference Period</h2>
          <div className="form-row">
            <div className="form-group">
              <label>Month *</label>
              <input
                type="number"
                name="referenceMonth"
                value={formData.referenceMonth}
                onChange={handleChange}
                min="1"
                max="12"
                required
              />
              {errors.referenceMonth && <span className="error">{errors.referenceMonth[0]}</span>}
            </div>
            <div className="form-group">
              <label>Year *</label>
              <input
                type="number"
                name="referenceYear"
                value={formData.referenceYear}
                onChange={handleChange}
                min="1960"
                required
              />
              {errors.referenceYear && <span className="error">{errors.referenceYear[0]}</span>}
            </div>
          </div>
        </div>

        <div className="form-section">
          <h2>Personal Information</h2>
          <div className="form-group">
            <label>Employee ID *</label>
            <input
              type="text"
              name="employeeId"
              value={formData.employeeId}
              onChange={handleChange}
              maxLength={5}
              required
            />
            {errors.employeeId && <span className="error">{errors.employeeId[0]}</span>}
          </div>

          <div className="form-group">
            <label>Full Name *</label>
            <input
              type="text"
              name="name"
              value={formData.name}
              onChange={handleChange}
              maxLength={35}
              required
            />
            {errors.name && <span className="error">{errors.name[0]}</span>}
          </div>

          <div className="form-group">
            <label>Position *</label>
            <input
              type="text"
              name="position"
              value={formData.position}
              onChange={handleChange}
              maxLength={30}
              required
            />
            {errors.position && <span className="error">{errors.position[0]}</span>}
          </div>

          <div className="form-group">
            <label>Company CNPJ *</label>
            <input
              type="text"
              name="cnpj"
              value={formData.cnpj}
              onChange={handleChange}
              placeholder="12345678000195"
              maxLength={14}
              required
            />
            {errors.cnpj && <span className="error">{errors.cnpj[0]}</span>}
          </div>

          <div className="form-group">
            <label>Hire Date *</label>
            <input
              type="date"
              name="hireDate"
              value={formData.hireDate}
              onChange={handleChange}
              required
            />
            {errors.hireDate && <span className="error">{errors.hireDate[0]}</span>}
          </div>
        </div>

        <div className="form-section">
          <h2>Salary Information</h2>
          <div className="form-row">
            <div className="form-group">
              <label>Base Salary (R$) *</label>
              <input
                type="number"
                name="baseSalary"
                value={formData.baseSalary}
                onChange={handleChange}
                step="0.01"
                min="0"
                required
              />
              {errors.baseSalary && <span className="error">{errors.baseSalary[0]}</span>}
            </div>

            <div className="form-group">
              <label>Working Hours *</label>
              <input
                type="number"
                name="workingHours"
                value={formData.workingHours}
                onChange={handleChange}
                min="0"
                required
              />
              {errors.workingHours && <span className="error">{errors.workingHours[0]}</span>}
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label>Overtime Hours</label>
              <input
                type="number"
                name="overtimeHours"
                value={formData.overtimeHours}
                onChange={handleChange}
                min="0"
              />
            </div>

            <div className="form-group">
              <label>Absences</label>
              <input
                type="number"
                name="absences"
                value={formData.absences}
                onChange={handleChange}
                min="0"
              />
            </div>
          </div>
        </div>

        <div className="form-section">
          <h2>Dependents and Benefits</h2>
          <div className="form-row">
            <div className="form-group">
              <label>Dependents</label>
              <input
                type="number"
                name="dependents"
                value={formData.dependents}
                onChange={handleChange}
                min="0"
              />
            </div>

            <div className="form-group">
              <label>Children</label>
              <input
                type="number"
                name="children"
                value={formData.children}
                onChange={handleChange}
                min="0"
              />
            </div>
          </div>

          <div className="form-group checkbox-group">
            <label>
              <input
                type="checkbox"
                name="optInTransportationVoucher"
                checked={formData.optInTransportationVoucher}
                onChange={handleChange}
              />
              <span>Opt for Transportation Voucher (6% of base salary)</span>
            </label>
          </div>
        </div>

        <div className="form-actions">
          <button
            type="button"
            onClick={() => navigate('/list')}
            className="btn btn-secondary"
          >
            Cancel
          </button>
          <button
            type="submit"
            className="btn btn-primary"
            disabled={createMutation.isPending || updateMutation.isPending}
          >
            {createMutation.isPending || updateMutation.isPending
              ? 'Saving...'
              : isEdit
              ? 'Update'
              : 'Create'}
          </button>
        </div>
      </form>
    </div>
  );
}