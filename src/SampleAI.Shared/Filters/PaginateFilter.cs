namespace SampleAI.Shared.Filters;

public record PaginateFilter
{
    public int PerPage { get; set; } = 20;
    public int CurrentPage { get; set; } = 0;

    public PaginateFilter(
        int perPage,
        int currentPage)
    {
        PerPage = perPage;
        CurrentPage = currentPage;
    }
}