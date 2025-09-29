namespace Demo.Security.Domain.Abstractions
{
    public interface IRepository<T, TId> where T : class, IEntity<TId>
    {
        Task<T?> GetByIdAsync(TId id, CancellationToken ct = default);
        Task AddAsync(T entity, CancellationToken ct = default);
    }
}
