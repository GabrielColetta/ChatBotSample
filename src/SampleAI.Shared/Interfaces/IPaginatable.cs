using SampleAI.Shared.Filters;
using SampleAI.Shared.Models;
using System.Linq.Expressions;

namespace SampleAI.Shared.Interfaces;

public interface IPaginatable<TEntity>
    where TEntity : class
{
    Task<PaginatedResponse<TResponse>> GetPaginatedAsync<TResponse>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResponse>> selector,
        PaginateFilter paginateFilter,
        CancellationToken cancellationToken)
        where TResponse : class;
}
