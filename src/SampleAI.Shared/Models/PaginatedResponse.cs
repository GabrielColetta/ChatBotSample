namespace SampleAI.Shared.Models;

public record PaginatedResponse<TModel>(IEnumerable<TModel> Data, int PerPage, int CurrentPage, int TotalItems);
