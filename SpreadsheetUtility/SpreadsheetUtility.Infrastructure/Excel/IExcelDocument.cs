using ClosedXML.Excel;

namespace SpreadsheetUtility.Infrastructure.Excel;

public interface IExcelDocument
{
    IXLWorksheet Worksheet(int index);
    void SaveAs(string filePath);
    IXLWorksheet AddWorksheet(string name);
    Task<string> GenerateOutputFileName();
}
