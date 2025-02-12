using SampleAI.Shared.Filters;
using SampleAI.Shared.Models;

namespace SampleAI.Shared.Interfaces;

public interface IDatabaseContext
{
    Task<IEnumerable<TDocument>> GetPaginatedAsync<TDocument>(string name, PaginateFilters<TDocument> paginateFilter, CancellationToken cancellationToken)
        where TDocument : class;

    Task<PaginatedResponse<TDocument>> GetSamplePaginatedAsync<TDocument>(string name, PaginateFilters<TDocument> paginateFilter, CancellationToken cancellationToken)
        where TDocument : class;

    Task InsertAsync<TDocument>(string name, TDocument document, CancellationToken cancellationToken) 
        where TDocument : class;
}
