namespace SpreadsheetUtility.Infrastructure.Options;

public class LlmOptions
{
    public const string SectionName = "Llm";
    public string BaseUrl { get; set; } = "http://localhost:1234";
    public string Model { get; set; } = "qwen2.5-3b-instruct";
    public int TimeoutSeconds { get; set; } = 60;
}
