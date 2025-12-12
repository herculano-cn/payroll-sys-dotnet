namespace Payroll.Application.Services;

/// <summary>
/// Service for validating Brazilian CNPJ (Cadastro Nacional da Pessoa Jurídica).
/// Preserves exact COBOL algorithm from VALIDA-CNPJ paragraph.
/// Maps to COBOL: VALIDA-CNPJ section
/// </summary>
public class CNPJValidationService : ICNPJValidationService
{
    /// <summary>
    /// Validates a CNPJ number using the exact algorithm from COBOL.
    /// </summary>
    /// <param name="cnpj">CNPJ string (can include formatting)</param>
    /// <returns>True if valid, false otherwise</returns>
    public bool ValidateCNPJ(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        // Remove formatting characters (dots, slashes, hyphens, spaces)
        // Maps to COBOL: INSPECT WS-CNPJ-NUM REPLACING ALL "." BY SPACES, etc.
        var cleanCnpj = CleanCNPJ(cnpj);

        // Check length (must be exactly 14 digits)
        // Maps to COBOL: IF FUNCTION LENGTH(WS-CNPJ-NUM) <> 14
        if (cleanCnpj.Length != 14)
            return false;

        // Check if all characters are numeric
        // Maps to COBOL: IF WS-CNPJ-NUM IS NOT NUMERIC
        if (!long.TryParse(cleanCnpj, out _))
            return false;

        // Check for known invalid CNPJs (all same digit)
        if (cleanCnpj.All(c => c == cleanCnpj[0]))
            return false;

        // Calculate and verify first check digit
        // Maps to COBOL: CALCULO DO PRIMEIRO DIGITO
        var firstDigit = CalculateFirstDigit(cleanCnpj);
        var expectedFirstDigit = int.Parse(cleanCnpj[12].ToString());

        if (firstDigit != expectedFirstDigit)
            return false;

        // Calculate and verify second check digit
        // Maps to COBOL: CALCULO DO SEGUNDO DIGITO
        var secondDigit = CalculateSecondDigit(cleanCnpj);
        var expectedSecondDigit = int.Parse(cleanCnpj[13].ToString());

        if (secondDigit != expectedSecondDigit)
            return false;

        return true;
    }

    /// <summary>
    /// Formats a CNPJ string with standard Brazilian formatting.
    /// Example: 12345678000195 -> 12.345.678/0001-95
    /// </summary>
    public string FormatCNPJ(string cnpj)
    {
        var clean = CleanCNPJ(cnpj);
        
        if (clean.Length != 14)
            return cnpj; // Return original if invalid length

        // Format: XX.XXX.XXX/XXXX-XX
        return $"{clean.Substring(0, 2)}.{clean.Substring(2, 3)}.{clean.Substring(5, 3)}/{clean.Substring(8, 4)}-{clean.Substring(12, 2)}";
    }

    /// <summary>
    /// Removes all formatting characters from CNPJ.
    /// Maps to COBOL: INSPECT WS-CNPJ-NUM REPLACING ALL...
    /// </summary>
    private string CleanCNPJ(string cnpj)
    {
        return new string(cnpj.Where(char.IsDigit).ToArray());
    }

    /// <summary>
    /// Calculates the first check digit using COBOL algorithm.
    /// Maps to COBOL: CALCULO DO PRIMEIRO DIGITO
    /// 
    /// COBOL Logic:
    /// COMPUTE WS-CNPJ-SOMA = 
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(1:1)) * 5 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(2:1)) * 4 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(3:1)) * 3 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(4:1)) * 2 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(5:1)) * 9 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(6:1)) * 8 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(7:1)) * 7 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(8:1)) * 6 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(9:1)) * 5 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(10:1)) * 4 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(11:1)) * 3 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(12:1)) * 2
    /// 
    /// DIVIDE WS-CNPJ-SOMA BY 11 GIVING WS-CNPJ-SOMA REMAINDER WS-CNPJ-RESTO
    /// 
    /// IF WS-CNPJ-RESTO < 2
    ///     MOVE 0 TO WS-CNPJ-DIG1
    /// ELSE
    ///     SUBTRACT WS-CNPJ-RESTO FROM 11 GIVING WS-CNPJ-DIG1
    /// END-IF.
    /// </summary>
    private int CalculateFirstDigit(string cnpj)
    {
        // Multipliers for first digit: 5,4,3,2,9,8,7,6,5,4,3,2
        int[] multipliers = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        
        int sum = 0;
        for (int i = 0; i < 12; i++)
        {
            sum += int.Parse(cnpj[i].ToString()) * multipliers[i];
        }

        int remainder = sum % 11;
        
        // If remainder < 2, digit is 0; otherwise, digit is 11 - remainder
        return remainder < 2 ? 0 : 11 - remainder;
    }

    /// <summary>
    /// Calculates the second check digit using COBOL algorithm.
    /// Maps to COBOL: CALCULO DO SEGUNDO DIGITO
    /// 
    /// COBOL Logic:
    /// COMPUTE WS-CNPJ-SOMA = 
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(1:1)) * 6 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(2:1)) * 5 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(3:1)) * 4 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(4:1)) * 3 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(5:1)) * 2 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(6:1)) * 9 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(7:1)) * 8 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(8:1)) * 7 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(9:1)) * 6 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(10:1)) * 5 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(11:1)) * 4 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(12:1)) * 3 +
    ///     FUNCTION NUMVAL(WS-CNPJ-NUM(13:1)) * 2
    /// 
    /// DIVIDE WS-CNPJ-SOMA BY 11 GIVING WS-CNPJ-SOMA REMAINDER WS-CNPJ-RESTO
    /// 
    /// IF WS-CNPJ-RESTO < 2
    ///     MOVE 0 TO WS-CNPJ-DIG2
    /// ELSE
    ///     SUBTRACT WS-CNPJ-RESTO FROM 11 GIVING WS-CNPJ-DIG2
    /// END-IF.
    /// </summary>
    private int CalculateSecondDigit(string cnpj)
    {
        // Multipliers for second digit: 6,5,4,3,2,9,8,7,6,5,4,3,2
        int[] multipliers = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        
        int sum = 0;
        for (int i = 0; i < 13; i++)
        {
            sum += int.Parse(cnpj[i].ToString()) * multipliers[i];
        }

        int remainder = sum % 11;
        
        // If remainder < 2, digit is 0; otherwise, digit is 11 - remainder
        return remainder < 2 ? 0 : 11 - remainder;
    }
}

/// <summary>
/// Interface for CNPJ validation service
/// </summary>
public interface ICNPJValidationService
{
    /// <summary>
    /// Validates a CNPJ number
    /// </summary>
    bool ValidateCNPJ(string cnpj);

    /// <summary>
    /// Formats a CNPJ with standard Brazilian formatting
    /// </summary>
    string FormatCNPJ(string cnpj);
}