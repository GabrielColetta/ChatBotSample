namespace SampleAI.AppHost.Configurations;

public class LanguageModelConfiguration
{
    public const string SectionName = "LanguageModel";

    public string Model { get; set; } = "deepseek-r1:14b";
    public int Port { get; set; }
}