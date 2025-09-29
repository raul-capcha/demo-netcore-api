namespace Demo.Security.Application.Result
{
    public sealed record Error(string Code, string Message)
    {
        public static readonly Error None = new("", "");
        public static Error Unauthorized(string message = "No autorizado") => new("auth.unauthorized", message);
        public static Error Conflict(string message) => new("common.conflict", message);
        public static Error NotFound(string message) => new("common.notfound", message);
        public static Error Validation(string message) => new("common.validation", message);
    }
}
