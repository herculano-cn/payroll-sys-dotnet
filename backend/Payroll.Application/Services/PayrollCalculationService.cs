using Payroll.Domain.Entities;

namespace Payroll.Application.Services;

/// <summary>
/// Service for calculating complete payroll including all earnings, deductions, and benefits.
/// Preserves exact COBOL calculation logic from CALCULOS section.
/// Maps to COBOL: CALCULOS paragraph
/// </summary>
public class PayrollCalculationService : IPayrollCalculationService
{
    private readonly ITaxCalculationService _taxCalculationService;

    public PayrollCalculationService(ITaxCalculationService taxCalculationService)
    {
        _taxCalculationService = taxCalculationService;
    }

    /// <summary>
    /// Calculates complete payroll for an employee.
    /// Maps to COBOL: CALCULOS section
    /// </summary>
    /// <param name="employee">Employee with input data</param>
    /// <returns>Employee with all calculated fields populated</returns>
    public Employee Calculate(Employee employee)
    {
        // 1. Calculate overtime pay
        // Maps to COBOL: COMPUTE FS-TOTAL-HE = ((FS-SALBA / FS-CH) + (FS-SALBA / FS-CH) * 50 / 100) * FS-HE
        employee.TotalOvertime = CalculateOvertime(
            employee.BaseSalary, 
            employee.WorkingHours, 
            employee.OvertimeHours);

        // 2. Calculate weekly rest (DSR)
        // Maps to COBOL: COMPUTE FS-DSR = (FS-TOTAL-HE / 26) * 4
        employee.WeeklyRest = CalculateWeeklyRest(employee.TotalOvertime);

        // 3. Calculate gross salary
        // Maps to COBOL: COMPUTE FS-SALBR = FS-SALBA + FS-TOTAL-HE + FS-DSR
        employee.GrossSalary = employee.BaseSalary + employee.TotalOvertime + employee.WeeklyRest;

        // 4. Calculate transportation voucher (if opted in)
        // Maps to COBOL: IF VT-SIM COMPUTE FS-VT = FS-SALBA * 0,06
        employee.TransportationVoucher = CalculateTransportationVoucher(
            employee.BaseSalary, 
            employee.OptInTransportationVoucher);

        // 5. Calculate INSS (Social Security)
        // Maps to COBOL: INICIO - CALCULANDO INSS
        employee.INSS = _taxCalculationService.CalculateINSS(employee.GrossSalary);

        // 6. Calculate dependent deduction
        // Maps to COBOL: COMPUTE FS-TOTAL-DEP = FS-DEP * 189,59
        employee.DependentDeduction = _taxCalculationService.CalculateDependentDeduction(employee.Dependents);

        // 7. Calculate IRRF (Income Tax)
        // Maps to COBOL: INICIO - CALCULANDO IRRF
        employee.IRRF = _taxCalculationService.CalculateIRRF(
            employee.GrossSalary, 
            employee.INSS, 
            employee.Dependents);

        // 8. Calculate family allowance (Salário Família)
        // Maps to COBOL: INICIO - CALCULANDO DO SALARIO FAMILIA
        employee.FamilyAllowance = CalculateFamilyAllowance(
            employee.BaseSalary, 
            employee.Children);

        // 9. Calculate absence deduction
        // Maps to COBOL: COMPUTE FS-TOTAL-FALTAS = (FS-SALBA / 30) * FS-FALTAS
        employee.AbsenceDeduction = CalculateAbsenceDeduction(
            employee.BaseSalary, 
            employee.Absences);

        // 10. Calculate FGTS (employer contribution, not deducted from salary)
        // Maps to COBOL: COMPUTE FS-FGTS = FS-SALBR * 0,08
        employee.FGTS = CalculateFGTS(employee.GrossSalary);

        // 11. Calculate net salary
        // Maps to COBOL: COMPUTE FS-SALIQ = FS-SALBR - FS-INSS - FS-IRRF - FS-VT - FS-TOTAL-FALTAS
        employee.NetSalary = employee.GrossSalary 
            - employee.INSS 
            - employee.IRRF 
            - employee.TransportationVoucher 
            - employee.AbsenceDeduction;

        return employee;
    }

    /// <summary>
    /// Calculates overtime pay with 50% premium.
    /// Maps to COBOL: COMPUTE FS-TOTAL-HE = ((FS-SALBA / FS-CH) + (FS-SALBA / FS-CH) * 50 / 100) * FS-HE
    /// 
    /// Example from SE docs:
    /// Base Salary: R$ 3,000.00
    /// Working Hours: 220
    /// Overtime Hours: 10
    /// Hourly Rate: R$ 3,000 / 220 = R$ 13.64
    /// Overtime Rate: R$ 13.64 * 1.5 = R$ 20.45
    /// Total Overtime: R$ 20.45 * 10 = R$ 204.55
    /// </summary>
    public decimal CalculateOvertime(decimal baseSalary, int workingHours, int overtimeHours)
    {
        if (workingHours == 0 || overtimeHours == 0)
            return 0;

        // Calculate hourly rate
        var hourlyRate = baseSalary / workingHours;

        // Overtime is paid at 150% (base rate + 50% premium)
        var overtimeRate = hourlyRate + (hourlyRate * 0.5m);

        // Total overtime pay
        return overtimeRate * overtimeHours;
    }

    /// <summary>
    /// Calculates weekly rest remuneration (DSR - Descanso Semanal Remunerado).
    /// Maps to COBOL: COMPUTE FS-DSR = (FS-TOTAL-HE / 26) * 4
    /// 
    /// DSR is calculated as a proportion of overtime pay:
    /// - 26 working days per month (average)
    /// - 4 Sundays/rest days per month
    /// </summary>
    public decimal CalculateWeeklyRest(decimal overtimePay)
    {
        if (overtimePay == 0)
            return 0;

        return (overtimePay / 26) * 4;
    }

    /// <summary>
    /// Calculates transportation voucher deduction (6% of base salary if opted in).
    /// Maps to COBOL: IF VT-SIM COMPUTE FS-VT = FS-SALBA * 0,06 ELSE COMPUTE FS-VT = 0
    /// </summary>
    public decimal CalculateTransportationVoucher(decimal baseSalary, bool optIn)
    {
        return optIn ? baseSalary * 0.06m : 0;
    }

    /// <summary>
    /// Calculates family allowance (Salário Família) based on salary and number of children.
    /// Maps to COBOL: INICIO - CALCULANDO DO SALARIO FAMILIA
    /// 
    /// COBOL Logic:
    /// EVALUATE TRUE
    ///     WHEN FS-FILHOS > 0 AND FS-SALBA <= 806,80
    ///         COMPUTE FS-TOTAL-SFM = 41,37 * FS-FILHOS
    ///     WHEN FS-SALBA > 806,81 AND FS-SALBA <= 1212,64 AND FS-FILHOS > 0
    ///         COMPUTE FS-TOTAL-SFM = 29,16 * FS-FILHOS
    /// END-EVALUATE.
    /// </summary>
    public decimal CalculateFamilyAllowance(decimal baseSalary, int children)
    {
        if (children == 0)
            return 0;

        // Brazilian family allowance rates (2024)
        return baseSalary switch
        {
            <= 806.80m => children * 41.37m,      // R$ 41.37 per child for low income
            <= 1212.64m => children * 29.16m,     // R$ 29.16 per child for mid income
            _ => 0                                 // No allowance for higher income
        };
    }

    /// <summary>
    /// Calculates deduction for absences (unpaid days).
    /// Maps to COBOL: COMPUTE FS-TOTAL-FALTAS = (FS-SALBA / 30) * FS-FALTAS
    /// 
    /// Assumes 30 days per month for calculation.
    /// </summary>
    public decimal CalculateAbsenceDeduction(decimal baseSalary, int absences)
    {
        if (absences == 0)
            return 0;

        // Daily rate = base salary / 30 days
        var dailyRate = baseSalary / 30;

        return dailyRate * absences;
    }

    /// <summary>
    /// Calculates FGTS (Fundo de Garantia do Tempo de Serviço).
    /// This is an employer contribution, not deducted from employee salary.
    /// Maps to COBOL: COMPUTE FS-FGTS = FS-SALBR * 0,08
    /// </summary>
    public decimal CalculateFGTS(decimal grossSalary)
    {
        // FGTS is 8% of gross salary
        return grossSalary * 0.08m;
    }
}

/// <summary>
/// Interface for payroll calculation service
/// </summary>
public interface IPayrollCalculationService
{
    /// <summary>
    /// Calculates complete payroll for an employee
    /// </summary>
    Employee Calculate(Employee employee);

    /// <summary>
    /// Calculates overtime pay
    /// </summary>
    decimal CalculateOvertime(decimal baseSalary, int workingHours, int overtimeHours);

    /// <summary>
    /// Calculates weekly rest remuneration
    /// </summary>
    decimal CalculateWeeklyRest(decimal overtimePay);

    /// <summary>
    /// Calculates transportation voucher deduction
    /// </summary>
    decimal CalculateTransportationVoucher(decimal baseSalary, bool optIn);

    /// <summary>
    /// Calculates family allowance
    /// </summary>
    decimal CalculateFamilyAllowance(decimal baseSalary, int children);

    /// <summary>
    /// Calculates absence deduction
    /// </summary>
    decimal CalculateAbsenceDeduction(decimal baseSalary, int absences);

    /// <summary>
    /// Calculates FGTS (employer contribution)
    /// </summary>
    decimal CalculateFGTS(decimal grossSalary);
}