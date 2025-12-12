using System.Net;
using System.Text.Json;
using Payroll.API.DTOs;
using Payroll.Domain.Exceptions;

namespace Payroll.API.Middleware;

/// <summary>
/// Global exception handling middleware.
/// Catches all exceptions and converts them to appropriate HTTP responses.
/// Maps to COBOL: Error handling in MOSTRA-ERRO paragraph
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Log the exception
        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        // Determine status code and response based on exception type
        var (statusCode, response) = exception switch
        {
            // Maps to COBOL: "PAYROLL NOT FOUND" error
            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                ApiResponse<object>.ErrorResponse(notFoundEx.Message)
            ),

            // Maps to COBOL: "INVALID FIELD(S)" error
            Domain.Exceptions.ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                ApiResponse<object>.ErrorResponse(
                    "One or more validation errors occurred.",
                    validationEx.Errors.ToDictionary(k => k.Key, v => v.Value)
                )
            ),

            // Maps to COBOL: "CLIENT ALREADY EXISTS" error
            DuplicateException duplicateEx => (
                HttpStatusCode.Conflict,
                ApiResponse<object>.ErrorResponse(duplicateEx.Message)
            ),

            // Generic validation exception (from FluentValidation)
            FluentValidation.ValidationException fluentValidationEx => (
                HttpStatusCode.BadRequest,
                ApiResponse<object>.ErrorResponse(
                    "One or more validation errors occurred.",
                    fluentValidationEx.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        )
                )
            ),

            // Unauthorized access
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                ApiResponse<object>.ErrorResponse("Unauthorized access.")
            ),

            // Generic server error
            _ => (
                HttpStatusCode.InternalServerError,
                ApiResponse<object>.ErrorResponse(
                    "An internal server error occurred. Please try again later."
                )
            )
        };

        // Set response
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        // Serialize and write response
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(response, jsonOptions);
        await context.Response.WriteAsync(json);
    }
}

/// <summary>
/// Extension method to register the exception handling middleware.
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}