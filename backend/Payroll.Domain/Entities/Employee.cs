namespace Payroll.Domain.Entities;

/// <summary>
/// Represents an employee payroll record.
/// Maps to COBOL: 01 FS-COLABORADOR
/// </summary>
public class Employee
{
    /// <summary>
    /// Primary key (auto-generated)
    /// </summary>
    public int Id { get; set; }

    // FS-CHAVE - Composite key fields
    /// <summary>
    /// Reference month (1-12)
    /// Maps to COBOL: FS-REF-MES PIC 99
    /// </summary>
    public int ReferenceMonth { get; set; }

    /// <summary>
    /// Reference year (>1959)
    /// Maps to COBOL: FS-REF-ANO PIC 9999
    /// </summary>
    public int ReferenceYear { get; set; }

    /// <summary>
    /// Employee ID (unique identifier)
    /// Maps to COBOL: FS-MAT PIC 9(5)
    /// </summary>
    public string EmployeeId { get; set; } = string.Empty;

    // FS-DADOS-ALFABETICOS - Personal information
    /// <summary>
    /// Employee full name (max 35 characters, alphabetic)
    /// Maps to COBOL: FS-NOME PIC X(35)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Employee position/role (max 30 characters, alphabetic)
    /// Maps to COBOL: FS-FUNC PIC X(30)
    /// </summary>
    public string Position { get; set; } = string.Empty;

    /// <summary>
    /// Company CNPJ (Brazilian tax ID, 14 digits)
    /// Maps to COBOL: FS-CNPJ PIC X(14)
    /// </summary>
    public string CNPJ { get; set; } = string.Empty;

    // FS-ADMISSAO - Hire date
    /// <summary>
    /// Hire date
    /// Maps to COBOL: FS-ADMISSAO (FS-ADM-DIA, FS-ADM-MES, FS-ADM-ANO)
    /// </summary>
    public DateTime HireDate { get; set; }

    // FS-DADOS-NUMERICOS - Numeric data
    /// <summary>
    /// Number of absences in the period
    /// Maps to COBOL: FS-FALTAS PIC 9(2)
    /// </summary>
    public int Absences { get; set; }

    /// <summary>
    /// Base salary (monthly)
    /// Maps to COBOL: FS-SALBA PIC 9(6)V9(2)
    /// CRITICAL: Use decimal for financial precision
    /// </summary>
    public decimal BaseSalary { get; set; }

    /// <summary>
    /// Working hours per month
    /// Maps to COBOL: FS-CH PIC 999
    /// </summary>
    public int WorkingHours { get; set; }

    /// <summary>
    /// Overtime hours
    /// Maps to COBOL: FS-HE PIC 99
    /// </summary>
    public int OvertimeHours { get; set; }

    /// <summary>
    /// Number of dependents (for tax calculation)
    /// Maps to COBOL: FS-DEP PIC 99
    /// </summary>
    public int Dependents { get; set; }

    /// <summary>
    /// Number of children (for family allowance)
    /// Maps to COBOL: FS-FILHOS PIC 99
    /// </summary>
    public int Children { get; set; }

    /// <summary>
    /// Opt-in for transportation voucher (Y/N)
    /// Maps to COBOL: FS-OP-VT PIC A (88 VT-SIM VALUE 'S')
    /// </summary>
    public bool OptInTransportationVoucher { get; set; }

    // Calculated fields (computed by PayrollCalculationService)
    /// <summary>
    /// Gross salary (base + overtime + weekly rest)
    /// Maps to COBOL: FS-SALBR PIC 9(6)V9(2)
    /// </summary>
    public decimal GrossSalary { get; set; }

    /// <summary>
    /// Net salary (gross - deductions)
    /// Maps to COBOL: FS-SALIQ PIC 9(6)V9(2)
    /// </summary>
    public decimal NetSalary { get; set; }

    /// <summary>
    /// Total overtime pay
    /// Maps to COBOL: FS-TOTAL-HE PIC 9(6)V9(2)
    /// </summary>
    public decimal TotalOvertime { get; set; }

    /// <summary>
    /// Weekly rest remuneration (DSR)
    /// Maps to COBOL: FS-DSR PIC 9(6)V9(2)
    /// </summary>
    public decimal WeeklyRest { get; set; }

    /// <summary>
    /// INSS (Social Security) deduction
    /// Maps to COBOL: FS-INSS PIC 9(6)V9(2)
    /// </summary>
    public decimal INSS { get; set; }

    /// <summary>
    /// IRRF (Income Tax) deduction
    /// Maps to COBOL: FS-IRRF PIC 9(6)V9(2)
    /// </summary>
    public decimal IRRF { get; set; }

    /// <summary>
    /// Family allowance (Salário Família)
    /// Maps to COBOL: FS-TOTAL-SFM PIC 9(6)V9(2)
    /// </summary>
    public decimal FamilyAllowance { get; set; }

    /// <summary>
    /// Dependent deduction value
    /// Maps to COBOL: FS-TOTAL-DEP PIC 9(6)V9(2)
    /// </summary>
    public decimal DependentDeduction { get; set; }

    /// <summary>
    /// Transportation voucher deduction
    /// Maps to COBOL: FS-VT PIC 9(6)V9(2)
    /// </summary>
    public decimal TransportationVoucher { get; set; }

    /// <summary>
    /// Total absence deduction
    /// Maps to COBOL: FS-TOTAL-FALTAS PIC 9(6)V9(2)
    /// </summary>
    public decimal AbsenceDeduction { get; set; }

    /// <summary>
    /// FGTS (Severance Fund) - employer contribution
    /// Maps to COBOL: FS-FGTS PIC 9(6)V9(2)
    /// </summary>
    public decimal FGTS { get; set; }

    // Audit fields
    /// <summary>
    /// Record creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Soft delete flag (for audit trail)
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Deletion timestamp
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}