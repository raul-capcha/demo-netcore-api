namespace Demo.Security.Domain.Abstractions
{
    public interface IEntity<TId>
    {
        TId Id { get; }
    }
}
