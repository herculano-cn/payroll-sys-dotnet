using Payroll.Domain.Entities;

namespace Payroll.Domain.Interfaces;

/// <summary>
/// Repository interface for Employee entity operations.
/// Abstracts data access layer from business logic.
/// </summary>
public interface IEmployeeRepository
{
    /// <summary>
    /// Gets an employee by their unique ID
    /// Maps to COBOL: READ FOLHA1
    /// </summary>
    Task<Employee?> GetByIdAsync(int id);

    /// <summary>
    /// Gets an employee by their employee ID (business key)
    /// Maps to COBOL: READ FOLHA1 with FS-MAT key
    /// </summary>
    Task<Employee?> GetByEmployeeIdAsync(string employeeId);

    /// <summary>
    /// Gets all employees (with optional filtering)
    /// Maps to COBOL: Sequential read of FOLHA1
    /// </summary>
    Task<IEnumerable<Employee>> GetAllAsync(bool includeDeleted = false);

    /// <summary>
    /// Gets employees by reference period
    /// </summary>
    Task<IEnumerable<Employee>> GetByReferencePeriodAsync(int month, int year);

    /// <summary>
    /// Adds a new employee
    /// Maps to COBOL: WRITE FS-COLABORADOR
    /// </summary>
    Task<Employee> AddAsync(Employee employee);

    /// <summary>
    /// Updates an existing employee
    /// Maps to COBOL: REWRITE FS-COLABORADOR
    /// </summary>
    Task<Employee> UpdateAsync(Employee employee);

    /// <summary>
    /// Deletes an employee (soft delete)
    /// Maps to COBOL: DELETE FOLHA1
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Checks if an employee exists by employee ID
    /// </summary>
    Task<bool> ExistsAsync(string employeeId);

    /// <summary>
    /// Saves all pending changes to the database
    /// Maps to COBOL: CLOSE FOLHA1 (commits changes)
    /// </summary>
    Task<int> SaveChangesAsync();
}