
namespace SampleAI.Shared.Interfaces;

public interface IDatabaseContext
{
    Task<IEnumerable<TDocument>> GetPaginatedAsync<TDocument>(string name, Filters.PaginateFilters<TDocument> paginateFilter) where TDocument : class;
    Task InsertAsync<TDocument>(string name, TDocument document) where TDocument : class;
}
