namespace SampleAI.Shared.Interfaces;

public interface IGettableByIdRepository<TEntity>
    where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
