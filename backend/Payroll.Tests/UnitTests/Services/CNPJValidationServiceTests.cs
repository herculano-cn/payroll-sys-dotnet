using FluentAssertions;
using Payroll.Application.Services;
using Xunit;

namespace Payroll.Tests.UnitTests.Services;

/// <summary>
/// Unit tests for CNPJ validation service.
/// Tests the exact COBOL algorithm implementation.
/// </summary>
public class CNPJValidationServiceTests
{
    private readonly CNPJValidationService _service;

    public CNPJValidationServiceTests()
    {
        _service = new CNPJValidationService();
    }

    [Theory]
    [InlineData("11222333000181")] // Valid CNPJ
    [InlineData("11.222.333/0001-81")] // Valid CNPJ with formatting
    public void ValidateCNPJ_ValidCNPJ_ReturnsTrue(string cnpj)
    {
        // Act
        var result = _service.ValidateCNPJ(cnpj);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("")] // Empty
    [InlineData("123")] // Too short
    [InlineData("12345678901234")] // Invalid checksum
    [InlineData("11111111111111")] // All same digit
    [InlineData("ABCD1234567890")] // Non-numeric
    public void ValidateCNPJ_InvalidCNPJ_ReturnsFalse(string cnpj)
    {
        // Act
        var result = _service.ValidateCNPJ(cnpj);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void FormatCNPJ_ValidCNPJ_ReturnsFormatted()
    {
        // Arrange
        var cnpj = "11222333000181";

        // Act
        var result = _service.FormatCNPJ(cnpj);

        // Assert
        result.Should().Be("11.222.333/0001-81");
    }
}