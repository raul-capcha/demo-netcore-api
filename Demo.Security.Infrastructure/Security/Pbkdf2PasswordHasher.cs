using Demo.Security.Infrastructure.Abstractions;
using System.Security.Cryptography;

namespace Demo.Security.Infrastructure.Security
{
    /// Formato almacenado: PBKDF2-SHA256$<iteraciones>$<saltBase64>$<hashBase64>
    public sealed class Pbkdf2PasswordHasher : IPasswordHasher
    {
        private const int Iterations = 100_000;
        private const int SaltSize = 16; // 128-bit
        private const int KeySize = 32;  // 256-bit

        public string Hash(string plaintext)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(plaintext, salt, Iterations, HashAlgorithmName.SHA256);
            var key = pbkdf2.GetBytes(KeySize);
            return $"PBKDF2-SHA256${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
        }

        public bool Verify(string hash, string plaintext)
        {
            try
            {
                var parts = hash.Split('$');
                if (parts.Length != 4 || parts[0] != "PBKDF2-SHA256") return false;

                var iterations = int.Parse(parts[1]);
                var salt = Convert.FromBase64String(parts[2]);
                var expectedKey = Convert.FromBase64String(parts[3]);

                using var pbkdf2 = new Rfc2898DeriveBytes(plaintext, salt, iterations, HashAlgorithmName.SHA256);
                var key = pbkdf2.GetBytes(expectedKey.Length);
                return CryptographicOperations.FixedTimeEquals(key, expectedKey);
            }
            catch { return false; }
        }
    }
}
