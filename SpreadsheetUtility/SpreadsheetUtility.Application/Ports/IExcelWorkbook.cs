namespace SpreadsheetUtility.Application.Ports;

public interface IExcelWorkbook
{
    IExcelWorksheet Worksheet(int index);
    Task<string> GenerateOutputFileName();
}
