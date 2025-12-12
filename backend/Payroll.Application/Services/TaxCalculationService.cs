namespace Payroll.Application.Services;

/// <summary>
/// Service for calculating Brazilian taxes (INSS and IRRF).
/// Preserves exact COBOL calculation logic.
/// Maps to COBOL: CALCULOS section (tax calculations)
/// </summary>
public class TaxCalculationService : ITaxCalculationService
{
    /// <summary>
    /// Calculates INSS (Social Security) using progressive rates.
    /// Maps to COBOL: INICIO - CALCULANDO INSS
    /// 
    /// COBOL Logic:
    /// EVALUATE TRUE
    ///     WHEN (FS-SALBR > 0) AND (FS-SALBR <= 1556,94)
    ///         COMPUTE FS-INSS = FS-SALBR * 0,08
    ///     WHEN (FS-SALBR > 1556,94) AND (FS-SALBR <= 2594,92)
    ///         COMPUTE FS-INSS = FS-SALBR * 0,09
    ///     WHEN (FS-SALBR > 2594,92) AND (FS-SALBR <= 5189,82)
    ///         COMPUTE FS-INSS = FS-SALBR * 0,11
    ///     WHEN (FS-SALBR > 5189,82)
    ///         COMPUTE FS-INSS = 5189,82 * 0,11
    /// END-EVALUATE.
    /// </summary>
    /// <param name="grossSalary">Gross salary (base + overtime + DSR)</param>
    /// <returns>INSS deduction amount</returns>
    public decimal CalculateINSS(decimal grossSalary)
    {
        // Brazilian INSS brackets (2024 rates)
        return grossSalary switch
        {
            <= 0 => 0,
            <= 1556.94m => grossSalary * 0.08m,      // 8% up to R$ 1,556.94
            <= 2594.92m => grossSalary * 0.09m,      // 9% from R$ 1,556.95 to R$ 2,594.92
            <= 5189.82m => grossSalary * 0.11m,      // 11% from R$ 2,594.93 to R$ 5,189.82
            _ => 570.88m                              // Fixed R$ 570.88 (11% of ceiling)
        };
    }

    /// <summary>
    /// Calculates IRRF (Income Tax) using progressive rates with deductions.
    /// Maps to COBOL: INICIO - CALCULANDO IRRF
    /// 
    /// COBOL Logic:
    /// First calculates dependent deduction:
    /// COMPUTE FS-TOTAL-DEP = FS-DEP * 189,59.
    /// 
    /// Then calculates taxable income and applies progressive rates:
    /// EVALUATE TRUE
    ///     WHEN ((FS-SALBR - FS-INSS) - FS-TOTAL-DEP > 1903,98) AND
    ///          (((FS-SALBR - FS-INSS) - FS-TOTAL-DEP) <= 2826,65)
    ///         COMPUTE FS-IRRF = (((FS-SALBR - FS-INSS) - FS-TOTAL-DEP) * 0,075) - 142,80
    ///     WHEN (((FS-SALBR - FS-INSS) - FS-TOTAL-DEP) > 2826,65) AND
    ///          (((FS-SALBR - FS-INSS) - (FS-DEP * 189,59)) <= 3751,05)
    ///         COMPUTE FS-IRRF = (((FS-SALBR - FS-INSS) - FS-TOTAL-DEP) * 0,150) - 354,80
    ///     WHEN (((FS-SALBR - FS-INSS) - FS-TOTAL-DEP) > 3751,05) AND
    ///          (((FS-SALBR - FS-INSS) - FS-TOTAL-DEP) <= 4664,68)
    ///         COMPUTE FS-IRRF = (((FS-SALBR - FS-INSS) - FS-TOTAL-DEP) * 0,225) - 636,13
    ///     WHEN (((FS-SALBR - FS-INSS) - FS-TOTAL-DEP) > 4664,68)
    ///         COMPUTE FS-IRRF = (((FS-SALBR - FS-INSS) - FS-TOTAL-DEP) * 0,275) - 869,36
    /// END-EVALUATE.
    /// </summary>
    /// <param name="grossSalary">Gross salary</param>
    /// <param name="inss">INSS deduction</param>
    /// <param name="dependents">Number of dependents</param>
    /// <returns>IRRF deduction amount</returns>
    public decimal CalculateIRRF(decimal grossSalary, decimal inss, int dependents)
    {
        // Calculate dependent deduction (R$ 189.59 per dependent)
        // Maps to COBOL: COMPUTE FS-TOTAL-DEP = FS-DEP * 189,59
        var dependentDeduction = dependents * 189.59m;

        // Calculate taxable income
        var taxableIncome = grossSalary - inss - dependentDeduction;

        // Brazilian IRRF brackets (2024 rates)
        // Exempt up to R$ 1,903.98
        if (taxableIncome <= 1903.98m)
            return 0;

        return taxableIncome switch
        {
            <= 2826.65m => taxableIncome * 0.075m - 142.80m,    // 7.5% - R$ 142.80
            <= 3751.05m => taxableIncome * 0.15m - 354.80m,     // 15% - R$ 354.80
            <= 4664.68m => taxableIncome * 0.225m - 636.13m,    // 22.5% - R$ 636.13
            _ => taxableIncome * 0.275m - 869.36m               // 27.5% - R$ 869.36
        };
    }

    /// <summary>
    /// Calculates the dependent deduction value for IRRF.
    /// Maps to COBOL: COMPUTE FS-TOTAL-DEP = FS-DEP * 189,59
    /// </summary>
    /// <param name="dependents">Number of dependents</param>
    /// <returns>Total dependent deduction</returns>
    public decimal CalculateDependentDeduction(int dependents)
    {
        // R$ 189.59 per dependent (2024 rate)
        return dependents * 189.59m;
    }
}

/// <summary>
/// Interface for tax calculation service
/// </summary>
public interface ITaxCalculationService
{
    /// <summary>
    /// Calculates INSS (Social Security) deduction
    /// </summary>
    decimal CalculateINSS(decimal grossSalary);

    /// <summary>
    /// Calculates IRRF (Income Tax) deduction
    /// </summary>
    decimal CalculateIRRF(decimal grossSalary, decimal inss, int dependents);

    /// <summary>
    /// Calculates dependent deduction for IRRF
    /// </summary>
    decimal CalculateDependentDeduction(int dependents);
}