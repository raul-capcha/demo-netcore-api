namespace Demo.Security.Infrastructure.Abstractions
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
