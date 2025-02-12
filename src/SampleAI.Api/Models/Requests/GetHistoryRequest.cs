namespace SampleAI.Api.Models.Requests;

public class GetHistoryRequest
{
    public uint PerPage { get; set; }
    public uint CurrentPage { get; set; }
}
