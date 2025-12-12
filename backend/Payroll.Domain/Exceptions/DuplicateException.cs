namespace Payroll.Domain.Exceptions;

/// <summary>
/// Exception thrown when attempting to create a duplicate record.
/// Maps to COBOL: "CLIENT ALREADY EXISTS" error message
/// HTTP Status: 409 Conflict
/// </summary>
public class DuplicateException : Exception
{
    public DuplicateException(string message) : base(message)
    {
    }

    public DuplicateException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public DuplicateException(string entityName, object key) 
        : base($"{entityName} with key '{key}' already exists.")
    {
    }
}