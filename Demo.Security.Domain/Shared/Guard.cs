namespace Demo.Security.Domain.Shared
{
    public static class Guard
    {
        public static void AgainstNullOrEmpty(string? value, string param)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{param} no puede ser vacío.", param);
        }
    }
}
