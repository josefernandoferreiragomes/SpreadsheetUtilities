namespace SpreadsheetUtility.Application.Ports;

public interface IExcelWorksheet
{
    string CellValue(int row, int col);
    int LastRowUsed();
}
