namespace SampleAI.Shared.Interfaces;

public interface ICreatableRepository<TEntity>
    where TEntity : class
{
    Task CreateAsync(TEntity entity, CancellationToken cancellationToken);
}
