using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Interfaces
{
    public interface IExcelWorkbook
    {
        IXLWorksheet Worksheet(int index);
        void SaveAs(string filePath);

        IXLWorksheet AddWorksheet(string name);

        Task<string> GenerateOutputFileName();
    }

}
