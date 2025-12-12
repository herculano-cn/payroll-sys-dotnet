using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Payroll.API.Middleware;
using Payroll.Application.Services;
using Payroll.Application.Validators;
using Payroll.Domain.Entities;
using Payroll.Domain.Interfaces;
using Payroll.Infrastructure.Data;
using Payroll.Infrastructure.Repositories;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// Controllers
builder.Services.AddControllers();

// Database - SQLite
builder.Services.AddDbContext<PayrollDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=payroll.db"
    )
);

// Repositories
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

// Services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IPayrollCalculationService, PayrollCalculationService>();
builder.Services.AddScoped<ITaxCalculationService, TaxCalculationService>();
builder.Services.AddScoped<ICNPJValidationService, CNPJValidationService>();

// Validators
builder.Services.AddScoped<IValidator<Employee>, EmployeeValidator>();

// CORS - Allow frontend to access API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",  // React dev server
                "http://localhost:5173"   // Vite dev server
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Payroll API",
        Version = "v1",
        Description = "API for managing employee payroll records. Modernized from COBOL legacy system.",
        Contact = new OpenApiContact
        {
            Name = "SE Modernizer",
            Email = "support@payroll.com"
        }
    });

    // Include XML comments for better documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Add JWT authentication to Swagger (for future use)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline

// Exception handling middleware (must be first)
app.UseExceptionHandling();

// Swagger (development and production)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Payroll API v1");
    options.RoutePrefix = "swagger"; // Access at /swagger
    options.DocumentTitle = "Payroll API Documentation";
});

// HTTPS redirection
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// CORS
app.UseCors("AllowFrontend");

// Authentication & Authorization (for future use)
// app.UseAuthentication();
// app.UseAuthorization();

// Map controllers
app.MapControllers();

// Database initialization
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PayrollDbContext>();
        
        // Create database if it doesn't exist
        // Maps to COBOL: OPEN OUTPUT FOLHA1 (creates file if not exists)
        context.Database.EnsureCreated();
        
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Database initialized successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database");
    }
}

// Welcome message
app.MapGet("/", () => Results.Ok(new
{
    message = "Payroll API is running",
    version = "1.0.0",
    documentation = "/swagger",
    endpoints = new
    {
        employees = "/api/employees",
        health = "/health"
    }
}))
.WithName("Root")
.WithTags("Info")
.Produces(200);

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow,
    database = "SQLite",
    environment = app.Environment.EnvironmentName
}))
.WithName("HealthCheck")
.WithTags("Info")
.Produces(200);

app.Run();