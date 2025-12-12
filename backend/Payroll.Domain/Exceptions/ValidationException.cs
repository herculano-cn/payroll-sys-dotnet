namespace Payroll.Domain.Exceptions;

/// <summary>
/// Exception thrown when validation fails.
/// Maps to COBOL: "INVALID FIELD(S)" error message
/// HTTP Status: 400 Bad Request
/// </summary>
public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException() 
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors) 
        : this()
    {
        Errors = errors;
    }

    public ValidationException(string field, string error) : this()
    {
        Errors[field] = new[] { error };
    }
}