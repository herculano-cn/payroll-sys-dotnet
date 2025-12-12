namespace Payroll.Domain.Exceptions;

/// <summary>
/// Exception thrown when a requested entity is not found.
/// Maps to COBOL: FS-NAO-EXISTE (file status 35)
/// HTTP Status: 404 Not Found
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public NotFoundException(string entityName, object key) 
        : base($"{entityName} with key '{key}' was not found.")
    {
    }
}