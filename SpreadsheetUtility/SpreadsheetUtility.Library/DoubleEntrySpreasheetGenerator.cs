using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using ClosedXML.Excel;
using Utilities.Interfaces;

namespace Utilities
{
    public class DoubleEntrySpreasheetGenerator
    {
        private const int WORKSHEET_INDEX_COUNT_DEFAULT = 1;
        private const int HEADER_ROW_INDEX_DEFAULT = 2;

        private readonly IExcelWorkbook _workbook;

        private string _keyColumnID;
        private string _valuesColumnID;
        private string _outputFilePath;
        private string _headersRow;
        private string _worksheetIndex;
        private int _keyColumnIDint;
        private int _valuesColumnIDint;
        private int _headersRowInt;
        private int _worksheetIndexInt;

        public DoubleEntrySpreasheetGenerator(IExcelWorkbook workbook, string keyColumnID, string valuesColumnID, string outputFilePath, string? headersRow = null, string? worksheetIndex = null)
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

        public async Task<List<string>> GenerateDoubleEntrySpreasheet()
        {
            var result = new List<string>();

            try
            {
                result.AddRange(ValidateInput());

                _outputFilePath = string.IsNullOrEmpty(_outputFilePath) ? await _workbook.GenerateOutputFileName().ConfigureAwait(false) : _outputFilePath;            

                var serviceMappings = await ReadInputFile();
                if (serviceMappings.Count == 0)
                {
                    result.Add("Error: No valid data found in input file.");
                    return result;
                }

                var uniqueThirdPartyServices = GetUniqueThirdPartyServices(serviceMappings);
                await WriteOutputFile(serviceMappings, uniqueThirdPartyServices).ConfigureAwait(false);

                result.Add($"Output file saved: {_outputFilePath}");
            }
            catch (Exception ex)
            {
                result.Add($"Error processing the file: {ex.Message}");
            }

            return result;
        }
        
        private List<string> ValidateInput()
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

        private async Task<Dictionary<string, HashSet<string>>> ReadInputFile()
        {            
            var serviceMappings = new Dictionary<string, HashSet<string>>();
            await Task.Run(() =>
            {  
                                 
                var worksheet = _workbook.Worksheet(_worksheetIndexInt);
                int rowCount = worksheet.LastRowUsed()?.RowNumber() ?? 0;


                for (int row = _headersRowInt; row <= rowCount; row++)
                {
                    string keyColumn = worksheet.Cell(row, _keyColumnIDint.ToString()).GetString().Trim();
                    string valuesColumn = worksheet.Cell(row, _valuesColumnIDint.ToString()).GetString().Trim();

                    if (string.IsNullOrEmpty(keyColumn)) continue;

                    if (!serviceMappings.ContainsKey(keyColumn))
                        serviceMappings[keyColumn] = new HashSet<string>();

                    foreach (var thirdPartyService in valuesColumn.Split(new[] { '\n', ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string service = thirdPartyService.Trim();
                        if (!string.IsNullOrEmpty(service))
                        {
                            serviceMappings[keyColumn].Add(service);
                        }
                    }
                }                          
            });            
            return serviceMappings;
        }      

        private HashSet<string> GetUniqueThirdPartyServices(Dictionary<string, HashSet<string>> serviceMappings)
        {
            var uniqueThirdPartyServices = new HashSet<string>();

            foreach (var entry in serviceMappings)
            {
                foreach (var service in entry.Value)
                {
                    uniqueThirdPartyServices.Add(service);
                }
            }

            return uniqueThirdPartyServices;
        }

        private async Task WriteOutputFile(Dictionary<string, HashSet<string>> serviceMappings, HashSet<string> uniqueThirdPartyServices)
        {
            await Task.Run(() =>
            {              
                var outputWorksheet = _workbook.AddWorksheet("Service Mapping");

                outputWorksheet.Cell(1, 1).Value = "keyColumn";
                int colIndex = 2;
                var serviceColumns = uniqueThirdPartyServices.OrderBy(s => s).ToList();

                foreach (var service in serviceColumns)
                {
                    outputWorksheet.Cell(1, colIndex++).Value = service;
                }

                int rowIndex = 2;
                foreach (var entry in serviceMappings)
                {
                    outputWorksheet.Cell(rowIndex, 1).Value = entry.Key;
                    colIndex = 2;

                    foreach (var service in serviceColumns)
                    {
                        if (entry.Value.Contains(service))
                        {
                            outputWorksheet.Cell(rowIndex, colIndex).Value = "X";
                        }
                        colIndex++;
                    }
                    rowIndex++;
                }

                _workbook.SaveAs(_outputFilePath);                
            });
        }        

    }

}
