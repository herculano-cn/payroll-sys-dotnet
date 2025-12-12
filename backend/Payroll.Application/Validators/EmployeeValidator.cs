using FluentValidation;
using Payroll.Application.Services;
using Payroll.Domain.Entities;

namespace Payroll.Application.Validators;

/// <summary>
/// Validator for Employee entity using FluentValidation.
/// Implements all validation rules from COBOL CALCULOS section.
/// Maps to COBOL: Input validation in CADASTRO-LOOP and CALCULOS
/// </summary>
public class EmployeeValidator : AbstractValidator<Employee>
{
    private readonly ICNPJValidationService _cnpjValidationService;

    public EmployeeValidator(ICNPJValidationService cnpjValidationService)
    {
        _cnpjValidationService = cnpjValidationService;

        // Reference Month validation
        // Maps to COBOL: IF FS-REF-MES < 1 OR FS-REF-MES > 12
        RuleFor(x => x.ReferenceMonth)
            .InclusiveBetween(1, 12)
            .WithMessage("Reference month must be between 1 and 12");

        // Reference Year validation
        // Maps to COBOL: IF FS-REF-ANO < 1959
        RuleFor(x => x.ReferenceYear)
            .GreaterThan(1959)
            .WithMessage("Reference year must be greater than 1959");

        // Employee ID validation
        // Maps to COBOL: FS-MAT PIC 9(5)
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("Employee ID is required")
            .MaximumLength(5)
            .WithMessage("Employee ID must not exceed 5 characters")
            .Matches(@"^\d+$")
            .WithMessage("Employee ID must contain only numbers");

        // Name validation
        // Maps to COBOL: FS-NOME = SPACES OR FS-NOME IS NOT ALPHABETIC
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(35)
            .WithMessage("Name must not exceed 35 characters")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("Name must contain only letters and spaces");

        // Position validation
        // Maps to COBOL: FS-FUNC = SPACES OR FS-FUNC IS NOT ALPHABETIC
        RuleFor(x => x.Position)
            .NotEmpty()
            .WithMessage("Position is required")
            .MaximumLength(30)
            .WithMessage("Position must not exceed 30 characters")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("Position must contain only letters and spaces");

        // CNPJ validation
        // Maps to COBOL: VALIDA-CNPJ section
        RuleFor(x => x.CNPJ)
            .NotEmpty()
            .WithMessage("CNPJ is required")
            .Must(BeValidCNPJ)
            .WithMessage("CNPJ is invalid");

        // Hire Date validation
        // Maps to COBOL: FS-ADM-DIA, FS-ADM-MES, FS-ADM-ANO validation
        RuleFor(x => x.HireDate)
            .NotEmpty()
            .WithMessage("Hire date is required")
            .Must(BeValidDate)
            .WithMessage("Hire date must be valid")
            .Must(BeAfter1959)
            .WithMessage("Hire date year must be greater than 1959")
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage("Hire date cannot be in the future");

        // Base Salary validation
        // Maps to COBOL: FS-SALBA = 0
        RuleFor(x => x.BaseSalary)
            .GreaterThan(0)
            .WithMessage("Base salary must be greater than zero")
            .LessThanOrEqualTo(999999.99m)
            .WithMessage("Base salary must not exceed R$ 999,999.99");

        // Working Hours validation
        // Maps to COBOL: FS-CH = 0
        RuleFor(x => x.WorkingHours)
            .GreaterThan(0)
            .WithMessage("Working hours must be greater than zero")
            .LessThanOrEqualTo(999)
            .WithMessage("Working hours must not exceed 999");

        // Overtime Hours validation
        // Maps to COBOL: FS-HE PIC 99
        RuleFor(x => x.OvertimeHours)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Overtime hours cannot be negative")
            .LessThanOrEqualTo(99)
            .WithMessage("Overtime hours must not exceed 99");

        // Absences validation
        // Maps to COBOL: FS-FALTAS PIC 9(2)
        RuleFor(x => x.Absences)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Absences cannot be negative")
            .LessThanOrEqualTo(99)
            .WithMessage("Absences must not exceed 99");

        // Dependents validation
        // Maps to COBOL: FS-DEP PIC 99
        RuleFor(x => x.Dependents)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Dependents cannot be negative")
            .LessThanOrEqualTo(99)
            .WithMessage("Dependents must not exceed 99");

        // Children validation
        // Maps to COBOL: FS-FILHOS PIC 99
        RuleFor(x => x.Children)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Children cannot be negative")
            .LessThanOrEqualTo(99)
            .WithMessage("Children must not exceed 99")
            .LessThanOrEqualTo(x => x.Dependents)
            .WithMessage("Children cannot exceed number of dependents");
    }

    /// <summary>
    /// Validates CNPJ using the COBOL algorithm.
    /// Maps to COBOL: VALIDA-CNPJ section
    /// </summary>
    private bool BeValidCNPJ(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        return _cnpjValidationService.ValidateCNPJ(cnpj);
    }

    /// <summary>
    /// Validates that date is valid (not default DateTime).
    /// </summary>
    private bool BeValidDate(DateTime date)
    {
        return date != default && date != DateTime.MinValue;
    }

    /// <summary>
    /// Validates that date year is after 1959.
    /// Maps to COBOL: IF FS-ADM-ANO < 1959
    /// </summary>
    private bool BeAfter1959(DateTime date)
    {
        return date.Year > 1959;
    }
}