using System.Text.RegularExpressions;

namespace ClassLib.Security;

public static class PasswordPolicy
{
    public const int MinimumLength = 10;
    public const int MaximumLength = 128;

    public static bool IsValid(string? password, out string error)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            error = "Adgangskoden skal udfyldes.";
            return false;
        }

        if (password.Length < MinimumLength)
        {
            error = $"Adgangskoden skal være mindst {MinimumLength} tegn.";
            return false;
        }

        if (password.Length > MaximumLength)
        {
            error = $"Adgangskoden må højst være {MaximumLength} tegn.";
            return false;
        }

        if (!password.Any(char.IsUpper))
        {
            error = "Adgangskoden skal indeholde mindst ét stort bogstav.";
            return false;
        }

        // A regex to check for special characters and numbers in the password. The regex should check for at least one special character and at least one number.
        // The special characters are: !@#$%^&*()_+-=[]{}|;':\",./<>?
        Regex specialCharacters = new Regex(@"[!@#$%^&*()_+\-=\[\]{}|;':"",./<>?]");
        if (!specialCharacters.IsMatch(password))
        {
            error = "Adgangskoden skal indeholde mindst ét specialtegn.";
            return false;
        }

        Regex numbers = new Regex(@"\d");
        if (!numbers.IsMatch(password))
        {
            error = "Adgangskoden skal indeholde mindst ét tal.";
            return false;
        }

        error = string.Empty;
        return true;
    }
}
