namespace Demo.Security.Application.Users
{
    public static class PasswordPolicy
    {
        public static string? Validate(string pwd)
        {
            if (string.IsNullOrWhiteSpace(pwd) || pwd.Length < 8) return "La contraseña debe tener al menos 8 caracteres.";
            bool hasUpper = pwd.Any(char.IsUpper);
            bool hasLower = pwd.Any(char.IsLower);
            bool hasDigit = pwd.Any(char.IsDigit);
            bool hasSymbol = pwd.Any(c => !char.IsLetterOrDigit(c));
            if (!hasUpper || !hasLower || !hasDigit || !hasSymbol)
                return "Debe incluir mayúsculas, minúsculas, dígitos y un símbolo.";
            return null;
        }
    }
}
