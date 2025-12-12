using FluentValidation;
using Payroll.Domain.Entities;
using Payroll.Domain.Exceptions;
using Payroll.Domain.Interfaces;

namespace Payroll.Application.Services;

/// <summary>
/// Service for managing employee payroll operations.
/// Implements all 4 user stories: Create, Read, Update, Delete.
/// Maps to COBOL: CADASTRO, PESQUISA, MODIFICA, DELETA paragraphs
/// </summary>
public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly IPayrollCalculationService _payrollCalculationService;
    private readonly IValidator<Employee> _validator;

    public EmployeeService(
        IEmployeeRepository repository,
        IPayrollCalculationService payrollCalculationService,
        IValidator<Employee> validator)
    {
        _repository = repository;
        _payrollCalculationService = payrollCalculationService;
        _validator = validator;
    }

    /// <summary>
    /// Creates a new employee payroll record.
    /// User Story: "Employee Registration in Payroll System"
    /// Maps to COBOL: CADASTRO paragraph
    /// </summary>
    public async Task<Employee> CreateAsync(Employee employee)
    {
        // Validate input
        // Maps to COBOL: Input validation in CADASTRO-LOOP
        var validationResult = await _validator.ValidateAsync(employee);
        if (!validationResult.IsValid)
        {
            // Log each validation error for debugging
            foreach (var error in validationResult.Errors)
            {
                Console.WriteLine($"Validation Error - Property: {error.PropertyName}, Error: {error.ErrorMessage}, AttemptedValue: {error.AttemptedValue}");
            }
            
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
            throw new Domain.Exceptions.ValidationException(errors);
        }

        // Check for duplicate employee ID
        // Maps to COBOL: WRITE FS-COLABORADOR INVALID KEY (duplicate check)
        var exists = await _repository.ExistsAsync(employee.EmployeeId);
        if (exists)
        {
            throw new DuplicateException("Employee", employee.EmployeeId);
        }

        // Calculate all payroll values
        // Maps to COBOL: PERFORM CALCULOS
        employee = _payrollCalculationService.Calculate(employee);

        // Set audit fields
        employee.CreatedAt = DateTime.UtcNow;
        employee.UpdatedAt = DateTime.UtcNow;
        employee.IsDeleted = false;

        // Save to database
        // Maps to COBOL: WRITE FS-COLABORADOR
        var created = await _repository.AddAsync(employee);
        await _repository.SaveChangesAsync();

        return created;
    }

    /// <summary>
    /// Gets an employee by their ID.
    /// User Story: "Search Employee in Payroll System"
    /// Maps to COBOL: PESQUISA paragraph
    /// </summary>
    public async Task<Employee> GetByIdAsync(int id)
    {
        // Maps to COBOL: READ FOLHA1
        var employee = await _repository.GetByIdAsync(id);

        if (employee == null || employee.IsDeleted)
        {
            // Maps to COBOL: "PAYROLL NOT FOUND" error
            throw new NotFoundException("Employee", id);
        }

        return employee;
    }

    /// <summary>
    /// Gets an employee by their employee ID (business key).
    /// User Story: "Search Employee in Payroll System"
    /// Maps to COBOL: PESQUISA paragraph with FS-MAT key
    /// </summary>
    public async Task<Employee> GetByEmployeeIdAsync(string employeeId)
    {
        // Maps to COBOL: READ FOLHA1 with FS-MAT
        var employee = await _repository.GetByEmployeeIdAsync(employeeId);

        if (employee == null || employee.IsDeleted)
        {
            // Maps to COBOL: "PAYROLL NOT FOUND" error
            throw new NotFoundException("Employee", employeeId);
        }

        return employee;
    }

    /// <summary>
    /// Gets all employees.
    /// Maps to COBOL: Sequential read of FOLHA1
    /// </summary>
    public async Task<IEnumerable<Employee>> GetAllAsync(bool includeDeleted = false)
    {
        return await _repository.GetAllAsync(includeDeleted);
    }

    /// <summary>
    /// Gets employees by reference period (month/year).
    /// </summary>
    public async Task<IEnumerable<Employee>> GetByReferencePeriodAsync(int month, int year)
    {
        if (month < 1 || month > 12)
            throw new Domain.Exceptions.ValidationException("Month", "Month must be between 1 and 12");

        if (year < 1960)
            throw new Domain.Exceptions.ValidationException("Year", "Year must be greater than 1959");

        return await _repository.GetByReferencePeriodAsync(month, year);
    }

    /// <summary>
    /// Updates an existing employee payroll record.
    /// User Story: "Modify Employee Payroll Information"
    /// Maps to COBOL: MODIFICA paragraph
    /// </summary>
    public async Task<Employee> UpdateAsync(Employee employee)
    {
        // Check if employee exists
        // Maps to COBOL: LE-CLIENTE (read for modification)
        var existing = await _repository.GetByIdAsync(employee.Id);
        if (existing == null || existing.IsDeleted)
        {
            throw new NotFoundException("Employee", employee.Id);
        }

        // Validate input
        // Maps to COBOL: Validation in MODIFICA
        var validationResult = await _validator.ValidateAsync(employee);
        if (!validationResult.IsValid)
        {
            // Log each validation error for debugging
            foreach (var error in validationResult.Errors)
            {
                Console.WriteLine($"Validation Error - Property: {error.PropertyName}, Error: {error.ErrorMessage}, AttemptedValue: {error.AttemptedValue}");
            }
            
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
            throw new Domain.Exceptions.ValidationException(errors);
        }

        // Check if employee ID changed and if new ID already exists
        if (employee.EmployeeId != existing.EmployeeId)
        {
            var exists = await _repository.ExistsAsync(employee.EmployeeId);
            if (exists)
            {
                throw new DuplicateException("Employee", employee.EmployeeId);
            }
        }

        // Update existing entity properties instead of creating new one
        // This avoids EF Core tracking conflicts
        existing.ReferenceMonth = employee.ReferenceMonth;
        existing.ReferenceYear = employee.ReferenceYear;
        existing.EmployeeId = employee.EmployeeId;
        existing.Name = employee.Name;
        existing.Position = employee.Position;
        existing.CNPJ = employee.CNPJ;
        existing.HireDate = employee.HireDate;
        existing.Absences = employee.Absences;
        existing.BaseSalary = employee.BaseSalary;
        existing.WorkingHours = employee.WorkingHours;
        existing.OvertimeHours = employee.OvertimeHours;
        existing.Dependents = employee.Dependents;
        existing.Children = employee.Children;
        existing.OptInTransportationVoucher = employee.OptInTransportationVoucher;

        // Recalculate all payroll values
        // Maps to COBOL: PERFORM CALCULOS in MODIFICA
        existing = _payrollCalculationService.Calculate(existing);

        // Update audit fields
        existing.UpdatedAt = DateTime.UtcNow;
        // CreatedAt and IsDeleted are already preserved in existing entity

        // Save changes
        // Maps to COBOL: REWRITE FS-COLABORADOR
        await _repository.SaveChangesAsync();

        return existing;
    }

    /// <summary>
    /// Deletes an employee payroll record (soft delete).
    /// User Story: "Delete Employee from Payroll System"
    /// Maps to COBOL: DELETA paragraph
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        // Check if employee exists
        // Maps to COBOL: LE-CLIENTE in DELETA
        var employee = await _repository.GetByIdAsync(id);
        if (employee == null || employee.IsDeleted)
        {
            // Maps to COBOL: "PAYROLL NOT FOUND" error
            throw new NotFoundException("Employee", id);
        }

        // Soft delete (mark as deleted instead of physical deletion)
        // Maps to COBOL: DELETE FOLHA1
        await _repository.DeleteAsync(id);
        await _repository.SaveChangesAsync();
    }
}

/// <summary>
/// Interface for Employee service
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// Creates a new employee (User Story 1)
    /// </summary>
    Task<Employee> CreateAsync(Employee employee);

    /// <summary>
    /// Gets an employee by ID (User Story 2)
    /// </summary>
    Task<Employee> GetByIdAsync(int id);

    /// <summary>
    /// Gets an employee by employee ID (User Story 2)
    /// </summary>
    Task<Employee> GetByEmployeeIdAsync(string employeeId);

    /// <summary>
    /// Gets all employees
    /// </summary>
    Task<IEnumerable<Employee>> GetAllAsync(bool includeDeleted = false);

    /// <summary>
    /// Gets employees by reference period
    /// </summary>
    Task<IEnumerable<Employee>> GetByReferencePeriodAsync(int month, int year);

    /// <summary>
    /// Updates an employee (User Story 3)
    /// </summary>
    Task<Employee> UpdateAsync(Employee employee);

    /// <summary>
    /// Deletes an employee (User Story 4)
    /// </summary>
    Task DeleteAsync(int id);
}