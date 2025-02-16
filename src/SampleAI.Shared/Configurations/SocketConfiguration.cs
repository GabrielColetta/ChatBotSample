namespace SampleAI.Shared.Configurations;

public class SocketConfiguration
{
    public string BaseUrl { get; set; } = "wss://localhost";
    public string Endpoint { get; set; } = "/chat";
}
