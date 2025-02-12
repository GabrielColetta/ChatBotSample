namespace SampleAI.Shared.Models;

public record PaginatedResponse<TModel>(IEnumerable<TModel> Data, uint PerPage, uint CurrentPage, uint TotalItems);
