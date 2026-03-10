using SampleAI.Shared.Filters;
using SampleAI.Shared.Models;
using System.Linq.Expressions;

namespace SampleAI.Shared.Interfaces;

public interface IPaginatable<TEntity>
    where TEntity : class
{
    Task<PaginatedResponse<TEntity>> GetPaginatedAsync(
        Expression<Func<TEntity, bool>> predicate,
        PaginateFilter paginateFilter,
        CancellationToken cancellationToken);
}
