using System.Linq.Expressions;

namespace SampleAI.Shared.Filters;

public record PaginateFilters<TData>
{
    public uint PerPage { get; set; } = 20;
    public uint CurrentPage { get; set; } = 0;

    public Expression<Func<TData, object>> SortByDesc { get; set; }
    public Expression<Func<TData, object>>? GroupBy { get; set; }
    public Expression<Func<TData, bool>>? FilterBy { get; set; }

    public PaginateFilters(
        uint perPage,
        uint currentPage,
        Expression<Func<TData, object>> sortByDesc, 
        Expression<Func<TData, object>>? groupBy = null,
        Expression<Func<TData, bool>>? filterBy = null)
    {
        PerPage = perPage;
        CurrentPage = currentPage;
        SortByDesc = sortByDesc;
        GroupBy = groupBy;
        FilterBy = filterBy;
    }
}
