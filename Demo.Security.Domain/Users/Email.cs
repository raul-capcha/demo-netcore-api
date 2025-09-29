using Demo.Security.Domain.Shared;
using System.Text.RegularExpressions;

namespace Demo.Security.Domain.Users
{
    public sealed class Email : IEquatable<Email>
    {
        private static readonly Regex Rx = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled);
        public string Value { get; }

        private Email(string value) => Value = value;

        public static Email Create(string value)
        {
            Guard.AgainstNullOrEmpty(value, nameof(Email));
            if (!Rx.IsMatch(value)) throw new ArgumentException("Email inválido.", nameof(Email));
            return new Email(value.Trim().ToLowerInvariant());
        }

        public override string ToString() => Value;
        public bool Equals(Email? other) => other is not null && Value == other.Value;
        public override bool Equals(object? obj) => obj is Email e && Equals(e);
        public override int GetHashCode() => Value.GetHashCode();
    }
}
