namespace SampleAI.Shared.Configurations;

public class DatabaseConfiguration
{
    public const string SectionName = "DatabaseConfiguration";
    public string Hostname { get; set; } = null!;
    public int Port { get; set; }
}
