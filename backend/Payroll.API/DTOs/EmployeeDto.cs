namespace Payroll.API.DTOs;

/// <summary>
/// Data Transfer Object for Employee entity.
/// Used for API requests and responses.
/// </summary>
public class EmployeeDto
{
    public int Id { get; set; }
    
    // Reference period
    public int ReferenceMonth { get; set; }
    public int ReferenceYear { get; set; }
    
    // Employee identification
    public string EmployeeId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string CNPJ { get; set; } = string.Empty;
    
    // Dates
    public DateTime HireDate { get; set; }
    
    // Work data
    public int Absences { get; set; }
    public decimal BaseSalary { get; set; }
    public int WorkingHours { get; set; }
    public int OvertimeHours { get; set; }
    
    // Family
    public int Dependents { get; set; }
    public int Children { get; set; }
    
    // Options
    public bool OptInTransportationVoucher { get; set; }
    
    // Calculated fields (read-only in responses)
    public decimal GrossSalary { get; set; }
    public decimal NetSalary { get; set; }
    public decimal TotalOvertime { get; set; }
    public decimal WeeklyRest { get; set; }
    public decimal INSS { get; set; }
    public decimal IRRF { get; set; }
    public decimal FamilyAllowance { get; set; }
    public decimal DependentDeduction { get; set; }
    public decimal TransportationVoucher { get; set; }
    public decimal AbsenceDeduction { get; set; }
    public decimal FGTS { get; set; }
    
    // Audit fields
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new employee (without calculated fields).
/// </summary>
public class CreateEmployeeDto
{
    public int ReferenceMonth { get; set; }
    public int ReferenceYear { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string CNPJ { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public int Absences { get; set; }
    public decimal BaseSalary { get; set; }
    public int WorkingHours { get; set; }
    public int OvertimeHours { get; set; }
    public int Dependents { get; set; }
    public int Children { get; set; }
    public bool OptInTransportationVoucher { get; set; }
}

/// <summary>
/// DTO for updating an employee (without calculated fields).
/// </summary>
public class UpdateEmployeeDto
{
    public int Id { get; set; }
    public int ReferenceMonth { get; set; }
    public int ReferenceYear { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string CNPJ { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public int Absences { get; set; }
    public decimal BaseSalary { get; set; }
    public int WorkingHours { get; set; }
    public int OvertimeHours { get; set; }
    public int Dependents { get; set; }
    public int Children { get; set; }
    public bool OptInTransportationVoucher { get; set; }
}

/// <summary>
/// Response wrapper for API responses.
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, Dictionary<string, string[]>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}