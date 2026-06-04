namespace SpreadsheetUtility.Application.DTOs;

public class GenerateDoubleEntryInput
{
    public string InputFilePath { get; set; } = string.Empty;
    public string KeyColumnId { get; set; } = string.Empty;
    public string ValuesColumnId { get; set; } = string.Empty;
    public string OutputFilePath { get; set; } = string.Empty;
    public string HeadersRow { get; set; } = string.Empty;
    public string WorksheetIndex { get; set; } = string.Empty;
}
