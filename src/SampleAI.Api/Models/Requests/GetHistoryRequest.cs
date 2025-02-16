namespace SampleAI.Api.Models.Requests;

public class GetHistoryRequest
{
    public uint PerPage { get; set; } = 20;
    public uint CurrentPage { get; set; } = 0;
}
