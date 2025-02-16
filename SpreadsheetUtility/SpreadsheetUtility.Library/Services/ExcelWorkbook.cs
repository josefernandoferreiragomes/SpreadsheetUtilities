using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Utilities.Services
{
    public class ExcelWorkbook : IExcelWorkbook
    {
        private string _filePath;
        private Lazy<XLWorkbook> _workbook;

        public ExcelWorkbook(string filePath)
        {            
            _filePath = filePath;            
            _workbook = new Lazy<XLWorkbook>(() => new XLWorkbook(_filePath));           
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

            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"Error: Input file '{_filePath}' not found.");
            }

            await Task.Run(() =>
            {
                string directory = Path.GetDirectoryName(_filePath) ?? ".";
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(_filePath);
                result = Path.Combine(directory, $"{fileNameWithoutExt}_output.xlsx");
            });
            return result;
        }
    }

}
