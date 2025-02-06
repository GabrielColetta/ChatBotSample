using System.Linq.Expressions;

namespace SampleAI.Shared.Filters;

public record PaginateFilters<TData>(int PerPage, Expression<Func<TData, object>> SortByDesc, Expression<Func<TData, object>> GroupBy);
