namespace Demo.Security.Infrastructure.Abstractions
{
    public interface IPasswordHasher
    {
        string Hash(string plaintext);
        bool Verify(string hash, string plaintext);
    }
}
