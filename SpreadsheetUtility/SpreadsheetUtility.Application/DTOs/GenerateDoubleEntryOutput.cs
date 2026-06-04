namespace SpreadsheetUtility.Application.DTOs;

public class GenerateDoubleEntryOutput
{
    public bool Success { get; set; }
    public List<string> Messages { get; set; } = new();
    public string OutputFilePath { get; set; } = string.Empty;
}
