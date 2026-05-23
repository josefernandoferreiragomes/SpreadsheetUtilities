using ClosedXML.Excel;

namespace SpreadsheetUtility.Library.Infrastructure;

public interface IExcelWorkbook
{
    IXLWorksheet Worksheet(int index);
    void SaveAs(string filePath);
    IXLWorksheet AddWorksheet(string name);
    Task<string> GenerateOutputFileName();
}
