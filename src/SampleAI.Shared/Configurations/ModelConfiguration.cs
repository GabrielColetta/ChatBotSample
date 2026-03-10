namespace SampleAI.Shared.Configurations;

public class ModelConfiguration
{
    public const string SectionName = "Model";

    public string Model { get; set; } = "";
    public string EmbeddingModel { get; set; } = "";
    public int Port { get; set; }
}