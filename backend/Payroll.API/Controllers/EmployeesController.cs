using Microsoft.AspNetCore.Mvc;
using Payroll.API.DTOs;
using Payroll.Application.Services;
using Payroll.Domain.Entities;

namespace Payroll.API.Controllers;

/// <summary>
/// Controller for employee payroll operations.
/// Implements all 4 user stories: Create, Read, Update, Delete.
/// Maps to COBOL: INICIO paragraph with menu options
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(
        IEmployeeService employeeService,
        ILogger<EmployeesController> logger)
    {
        _employeeService = employeeService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new employee payroll record.
    /// User Story 1: "Employee Registration in Payroll System"
    /// Maps to COBOL: CADASTRAR option (1)
    /// </summary>
    /// <param name="dto">Employee data</param>
    /// <returns>Created employee with calculated payroll</returns>
    /// <response code="201">Employee created successfully</response>
    /// <response code="400">Invalid input data</response>
    /// <response code="409">Employee ID already exists</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> CreateEmployee(
        [FromBody] CreateEmployeeDto dto)
    {
        _logger.LogInformation("Creating employee with ID: {EmployeeId}", dto.EmployeeId);
        _logger.LogInformation("Received data - Month: {Month}, Year: {Year}, Name: '{Name}', CNPJ: '{CNPJ}', HireDate: '{HireDate}', BaseSalary: {BaseSalary}, WorkingHours: {WorkingHours}",
            dto.ReferenceMonth, dto.ReferenceYear, dto.Name, dto.CNPJ, dto.HireDate, dto.BaseSalary, dto.WorkingHours);

        // Map DTO to Entity
        var employee = MapToEntity(dto);
        
        _logger.LogInformation("Mapped entity - HireDate type: {Type}, Value: '{Value}', IsDefault: {IsDefault}",
            employee.HireDate.GetType().Name, employee.HireDate, employee.HireDate == default);

        // Create employee (service will validate and calculate payroll)
        var created = await _employeeService.CreateAsync(employee);

        // Map back to DTO
        var responseDto = MapToDto(created);

        var response = ApiResponse<EmployeeDto>.SuccessResponse(
            responseDto,
            "Employee created successfully"
        );

        return CreatedAtAction(
            nameof(GetEmployeeById),
            new { id = created.Id },
            response
        );
    }

    /// <summary>
    /// Gets an employee by their ID.
    /// User Story 2: "Search Employee in Payroll System"
    /// Maps to COBOL: PESQUISAR option (2)
    /// </summary>
    /// <param name="id">Employee database ID</param>
    /// <returns>Employee payroll record</returns>
    /// <response code="200">Employee found</response>
    /// <response code="404">Employee not found</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetEmployeeById(int id)
    {
        _logger.LogInformation("Getting employee by ID: {Id}", id);

        var employee = await _employeeService.GetByIdAsync(id);
        var dto = MapToDto(employee);

        var response = ApiResponse<EmployeeDto>.SuccessResponse(dto);
        return Ok(response);
    }

    /// <summary>
    /// Gets an employee by their employee ID (business key).
    /// User Story 2: "Search Employee in Payroll System"
    /// Maps to COBOL: PESQUISAR option (2) with FS-MAT key
    /// </summary>
    /// <param name="employeeId">Employee ID (business key)</param>
    /// <returns>Employee payroll record</returns>
    /// <response code="200">Employee found</response>
    /// <response code="404">Employee not found</response>
    [HttpGet("by-employee-id/{employeeId}")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetEmployeeByEmployeeId(
        string employeeId)
    {
        _logger.LogInformation("Getting employee by employee ID: {EmployeeId}", employeeId);

        var employee = await _employeeService.GetByEmployeeIdAsync(employeeId);
        var dto = MapToDto(employee);

        var response = ApiResponse<EmployeeDto>.SuccessResponse(dto);
        return Ok(response);
    }

    /// <summary>
    /// Gets all employees.
    /// Maps to COBOL: Sequential read of FOLHA1
    /// </summary>
    /// <param name="includeDeleted">Include soft-deleted records</param>
    /// <returns>List of all employees</returns>
    /// <response code="200">Employees retrieved successfully</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeDto>>>> GetAllEmployees(
        [FromQuery] bool includeDeleted = false)
    {
        _logger.LogInformation("Getting all employees (includeDeleted: {IncludeDeleted})", includeDeleted);

        var employees = await _employeeService.GetAllAsync(includeDeleted);
        var dtos = employees.Select(MapToDto);

        var response = ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(dtos);
        return Ok(response);
    }

    /// <summary>
    /// Gets employees by reference period (month/year).
    /// </summary>
    /// <param name="month">Reference month (1-12)</param>
    /// <param name="year">Reference year</param>
    /// <returns>List of employees for the specified period</returns>
    /// <response code="200">Employees retrieved successfully</response>
    /// <response code="400">Invalid month or year</response>
    [HttpGet("by-period")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeDto>>>> GetEmployeesByPeriod(
        [FromQuery] int month,
        [FromQuery] int year)
    {
        _logger.LogInformation("Getting employees for period: {Month}/{Year}", month, year);

        var employees = await _employeeService.GetByReferencePeriodAsync(month, year);
        var dtos = employees.Select(MapToDto);

        var response = ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(dtos);
        return Ok(response);
    }

    /// <summary>
    /// Updates an existing employee payroll record.
    /// User Story 3: "Modify Employee Payroll Information"
    /// Maps to COBOL: MODIFICAR option (3)
    /// </summary>
    /// <param name="id">Employee database ID</param>
    /// <param name="dto">Updated employee data</param>
    /// <returns>Updated employee with recalculated payroll</returns>
    /// <response code="200">Employee updated successfully</response>
    /// <response code="400">Invalid input data</response>
    /// <response code="404">Employee not found</response>
    /// <response code="409">Employee ID already exists (if changed)</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> UpdateEmployee(
        int id,
        [FromBody] UpdateEmployeeDto dto)
    {
        _logger.LogInformation("Updating employee ID: {Id}", id);

        // Ensure ID matches
        if (id != dto.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(
                "ID in URL does not match ID in request body"
            ));
        }

        // Map DTO to Entity
        var employee = MapToEntity(dto);

        // Update employee (service will validate and recalculate payroll)
        var updated = await _employeeService.UpdateAsync(employee);

        // Map back to DTO
        var responseDto = MapToDto(updated);

        var response = ApiResponse<EmployeeDto>.SuccessResponse(
            responseDto,
            "Employee updated successfully"
        );

        return Ok(response);
    }

    /// <summary>
    /// Deletes an employee payroll record (soft delete).
    /// User Story 4: "Delete Employee from Payroll System"
    /// Maps to COBOL: DELETAR option (4)
    /// </summary>
    /// <param name="id">Employee database ID</param>
    /// <returns>Success message</returns>
    /// <response code="200">Employee deleted successfully</response>
    /// <response code="404">Employee not found</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteEmployee(int id)
    {
        _logger.LogInformation("Deleting employee ID: {Id}", id);

        await _employeeService.DeleteAsync(id);

        var response = ApiResponse<object>.SuccessResponse(
            null,
            "Employee deleted successfully"
        );

        return Ok(response);
    }

    #region Mapping Methods

    /// <summary>
    /// Maps CreateEmployeeDto to Employee entity.
    /// </summary>
    private Employee MapToEntity(CreateEmployeeDto dto)
    {
        return new Employee
        {
            ReferenceMonth = dto.ReferenceMonth,
            ReferenceYear = dto.ReferenceYear,
            EmployeeId = dto.EmployeeId,
            Name = dto.Name,
            Position = dto.Position,
            CNPJ = dto.CNPJ,
            HireDate = dto.HireDate,
            Absences = dto.Absences,
            BaseSalary = dto.BaseSalary,
            WorkingHours = dto.WorkingHours,
            OvertimeHours = dto.OvertimeHours,
            Dependents = dto.Dependents,
            Children = dto.Children,
            OptInTransportationVoucher = dto.OptInTransportationVoucher
        };
    }

    /// <summary>
    /// Maps UpdateEmployeeDto to Employee entity.
    /// </summary>
    private Employee MapToEntity(UpdateEmployeeDto dto)
    {
        return new Employee
        {
            Id = dto.Id,
            ReferenceMonth = dto.ReferenceMonth,
            ReferenceYear = dto.ReferenceYear,
            EmployeeId = dto.EmployeeId,
            Name = dto.Name,
            Position = dto.Position,
            CNPJ = dto.CNPJ,
            HireDate = dto.HireDate,
            Absences = dto.Absences,
            BaseSalary = dto.BaseSalary,
            WorkingHours = dto.WorkingHours,
            OvertimeHours = dto.OvertimeHours,
            Dependents = dto.Dependents,
            Children = dto.Children,
            OptInTransportationVoucher = dto.OptInTransportationVoucher
        };
    }

    /// <summary>
    /// Maps Employee entity to EmployeeDto.
    /// </summary>
    private EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            ReferenceMonth = employee.ReferenceMonth,
            ReferenceYear = employee.ReferenceYear,
            EmployeeId = employee.EmployeeId,
            Name = employee.Name,
            Position = employee.Position,
            CNPJ = employee.CNPJ,
            HireDate = employee.HireDate,
            Absences = employee.Absences,
            BaseSalary = employee.BaseSalary,
            WorkingHours = employee.WorkingHours,
            OvertimeHours = employee.OvertimeHours,
            Dependents = employee.Dependents,
            Children = employee.Children,
            OptInTransportationVoucher = employee.OptInTransportationVoucher,
            GrossSalary = employee.GrossSalary,
            NetSalary = employee.NetSalary,
            TotalOvertime = employee.TotalOvertime,
            WeeklyRest = employee.WeeklyRest,
            INSS = employee.INSS,
            IRRF = employee.IRRF,
            FamilyAllowance = employee.FamilyAllowance,
            DependentDeduction = employee.DependentDeduction,
            TransportationVoucher = employee.TransportationVoucher,
            AbsenceDeduction = employee.AbsenceDeduction,
            FGTS = employee.FGTS,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt
        };
    }

    #endregion
}