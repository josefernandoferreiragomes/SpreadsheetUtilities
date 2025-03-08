using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using ClosedXML.Excel;
using Utilities.Interfaces;

namespace Utilities
{
    public abstract class SpreasheetGeneratorBase
    {
        internal const int WORKSHEET_INDEX_COUNT_DEFAULT = 1;
        internal const int HEADER_ROW_INDEX_DEFAULT = 2;
        internal readonly IExcelWorkbook _workbook;
        internal string _keyColumnID;
        internal string _valuesColumnID;
        internal string _outputFilePath;
        internal string _headersRow;
        internal string _worksheetIndex;
        internal int _keyColumnIDint;
        internal int _valuesColumnIDint;
        internal int _headersRowInt;
        internal int _worksheetIndexInt;

        public SpreasheetGeneratorBase(IExcelWorkbook workbook, string keyColumnID, string valuesColumnID, string outputFilePath, string? headersRow = null, string? worksheetIndex = null)
        {
            _workbook = workbook;
           
            _keyColumnID = keyColumnID;
            _valuesColumnID = valuesColumnID;
            _outputFilePath = outputFilePath;
            _headersRow = headersRow ?? string.Empty;
            _worksheetIndex = worksheetIndex ?? string.Empty;            
            _worksheetIndexInt = int.TryParse(_worksheetIndex, out _worksheetIndexInt) ? _worksheetIndexInt : WORKSHEET_INDEX_COUNT_DEFAULT;
            _headersRowInt = int.TryParse(_headersRow, out _headersRowInt) ? _headersRowInt : HEADER_ROW_INDEX_DEFAULT;
        }

        public abstract Task<List<string>> Generate();
        protected abstract Task WriteOutputFile(Dictionary<string, HashSet<string>> serviceMappings, HashSet<string> uniqueThirdPartyServices);

        protected virtual List<string> ValidateInput()
        {
            var result = new List<string>();           
            if (string.IsNullOrEmpty(_keyColumnID) || !int.TryParse(_keyColumnID, out _keyColumnIDint))
            {
                result.Add("Error: Key Column ID is missing.");
                return result;
            }

            if (string.IsNullOrEmpty(_valuesColumnID) || !int.TryParse(_valuesColumnID, out _valuesColumnIDint))
            {
                result.Add("Error: Values Column ID is missing.");
                return result;
            }
            return result;
        }              

        protected HashSet<string> GetSplitCellRows(Dictionary<string, HashSet<string>> keyValueDictionary)
        {
            var uniqueValuesForKeys = new HashSet<string>();

            foreach (var entry in keyValueDictionary)
            {
                foreach (var service in entry.Value)
                {
                    uniqueValuesForKeys.Add(service);
                }
            }

            return uniqueValuesForKeys;
        }
    }

}
