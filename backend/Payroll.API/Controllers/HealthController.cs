using Microsoft.AspNetCore.Mvc;

namespace Payroll.API.Controllers;

/// <summary>
/// Health check endpoint for monitoring and load balancers
/// </summary>
[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        _logger.LogDebug("Health check requested");
        
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            service = "Payroll API",
            version = "1.0.0"
        });
    }
}