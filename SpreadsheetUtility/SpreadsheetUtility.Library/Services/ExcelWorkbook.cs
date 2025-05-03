using ClosedXML.Excel;
using Utilities.Interfaces;

namespace Utilities.Services
{
    public class ExcelWorkbook : IExcelWorkbook
    {
        private string _filePath;
        private Lazy<IXLWorkbook> _workbook;

        public ExcelWorkbook(string filePath)
        {
            try
            {
                _filePath = filePath;
                _workbook = new Lazy<IXLWorkbook>(() => new XLWorkbook(_filePath));
            }
            catch (Exception ex)
            {
                //TODO: Property Log the exception
                Console.Write(ex.ToString());
                throw;
            }
        }

        public ExcelWorkbook(string filePath, Lazy<IXLWorkbook> workbook)
        {
            _filePath = filePath;
            _workbook = workbook;
        }

        public IXLWorksheet Worksheet(int index)
        {
            return _workbook.Value.Worksheet(index);
        }

        public IXLWorksheet AddWorksheet(string name)
        {
            return _workbook.Value.AddWorksheet(name);
        }

        public void SaveAs(string filePath)
        {
            _workbook.Value.SaveAs(filePath);
        }

        public async Task<string> GenerateOutputFileName()
        {
            string result = string.Empty;
            ValidateFilePath();

            await Task.Run(() =>
            {
                string directory = Path.GetDirectoryName(_filePath) ?? ".";
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(_filePath);
                result = Path.Combine(directory, $"{fileNameWithoutExt}_output.xlsx");
            });
            return result;
        }

        private void ValidateFilePath()
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"Error: Input file '{_filePath}' not found.");
            }
        }
    }

}
