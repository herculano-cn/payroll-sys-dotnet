using Microsoft.EntityFrameworkCore;
using Payroll.Domain.Entities;
using Payroll.Domain.Interfaces;
using Payroll.Infrastructure.Data;

namespace Payroll.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Employee entity.
/// Maps to COBOL: File operations on FOLHA1
/// </summary>
public class EmployeeRepository : IEmployeeRepository
{
    private readonly PayrollDbContext _context;

    public EmployeeRepository(PayrollDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets an employee by ID.
    /// Maps to COBOL: READ FOLHA1
    /// </summary>
    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.Employees
            .IgnoreQueryFilters() // Include soft-deleted records
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    /// <summary>
    /// Gets an employee by employee ID (business key).
    /// Maps to COBOL: READ FOLHA1 with FS-MAT key
    /// </summary>
    public async Task<Employee?> GetByEmployeeIdAsync(string employeeId)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
    }

    /// <summary>
    /// Gets all employees.
    /// Maps to COBOL: Sequential read of FOLHA1
    /// </summary>
    public async Task<IEnumerable<Employee>> GetAllAsync(bool includeDeleted = false)
    {
        var query = _context.Employees.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return await query
            .OrderBy(e => e.ReferenceYear)
            .ThenBy(e => e.ReferenceMonth)
            .ThenBy(e => e.EmployeeId)
            .ToListAsync();
    }

    /// <summary>
    /// Gets employees by reference period.
    /// </summary>
    public async Task<IEnumerable<Employee>> GetByReferencePeriodAsync(int month, int year)
    {
        return await _context.Employees
            .Where(e => e.ReferenceMonth == month && e.ReferenceYear == year)
            .OrderBy(e => e.EmployeeId)
            .ToListAsync();
    }

    /// <summary>
    /// Adds a new employee.
    /// Maps to COBOL: WRITE FS-COLABORADOR
    /// </summary>
    public async Task<Employee> AddAsync(Employee employee)
    {
        await _context.Employees.AddAsync(employee);
        return employee;
    }

    /// <summary>
    /// Updates an existing employee.
    /// Maps to COBOL: REWRITE FS-COLABORADOR
    /// </summary>
    public async Task<Employee> UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);
        return employee;
    }

    /// <summary>
    /// Deletes an employee (soft delete).
    /// Maps to COBOL: DELETE FOLHA1
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        var employee = await GetByIdAsync(id);
        if (employee != null)
        {
            employee.IsDeleted = true;
            employee.DeletedAt = DateTime.UtcNow;
            _context.Employees.Update(employee);
        }
    }

    /// <summary>
    /// Checks if an employee exists by employee ID.
    /// </summary>
    public async Task<bool> ExistsAsync(string employeeId)
    {
        return await _context.Employees
            .AnyAsync(e => e.EmployeeId == employeeId);
    }

    /// <summary>
    /// Saves all pending changes.
    /// Maps to COBOL: CLOSE FOLHA1 (commits changes)
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}