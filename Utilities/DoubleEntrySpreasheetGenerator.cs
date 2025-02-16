using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;

namespace Utilities
{
    public class DoubleEntrySpreasheetGenerator
    {
        private const int WORKSHEET_INDEX_COUNT_DEFAULT = 1;
        private const int HEADER_ROW_INDEX_DEFAULT = 2;
        private string _inputFilePath;
        private string _keyColumnID;
        private string _valuesColumnID;
        private string _outputFilePath;
        private string _headersRow;
        private string _worksheetIndex;
        private int _keyColumnIDint;
        private int _valuesColumnIDint;
        private int _headersRowInt;
        private int _worksheetIndexInt;

        public DoubleEntrySpreasheetGenerator(string inputFilePath, string keyColumnID, string valuesColumnID, string outputFilePath, string? headersRow = null, string? worksheetIndex = null)
        {
            _inputFilePath = inputFilePath;
            _keyColumnID = keyColumnID;
            _valuesColumnID = valuesColumnID;
            _outputFilePath = outputFilePath;
            _headersRow = headersRow ?? string.Empty;
            _worksheetIndex = worksheetIndex ?? string.Empty;
            ValidateInput();
            _worksheetIndexInt = int.TryParse(_worksheetIndex, out _worksheetIndexInt) ? _worksheetIndexInt : WORKSHEET_INDEX_COUNT_DEFAULT;
            _headersRowInt = int.TryParse(_headersRow, out _headersRowInt) ? _headersRowInt : HEADER_ROW_INDEX_DEFAULT;
        }        

        public async Task<List<string>> GenerateDoubleEntrySpreasheet()
        {
            var result = new List<string>();
            
            if(string.IsNullOrEmpty(_keyColumnID) || !int.TryParse(_keyColumnID, out _keyColumnIDint) || !int.TryParse(_valuesColumnID, out _valuesColumnIDint))
            {
                result.Add("Error: Column ID is missing.");
                return result;
            }

            _outputFilePath = string.IsNullOrEmpty(_outputFilePath) ? await GenerateOutputFileName(_inputFilePath).ConfigureAwait(false) : _outputFilePath;

            if (!File.Exists(_inputFilePath))
            {
                result.Add($"Error: Input file '{_inputFilePath}' not found.");
                return result;
            }

            try
            {
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

        //This method is used to validate the input, namely types and values of the input parameters.        
        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(_inputFilePath) || string.IsNullOrEmpty(_keyColumnID) || string.IsNullOrEmpty(_valuesColumnID))
            {
                return false;
            }
            if (!int.TryParse(_keyColumnID, out _keyColumnIDint) || !int.TryParse(_valuesColumnID, out _valuesColumnIDint))
            {
                return false;
            }
            return true;
        }

        private async Task<Dictionary<string, HashSet<string>>> ReadInputFile()
        {            
            var serviceMappings = new Dictionary<string, HashSet<string>>();
            await Task.Run(() =>
            {  
                using (var workbook = new XLWorkbook(_inputFilePath))
                {                    
                    var worksheet = workbook.Worksheet(_worksheetIndexInt);
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
                using (var outputWorkbook = new XLWorkbook())
                {
                    var outputWorksheet = outputWorkbook.Worksheets.Add("Service Mapping");

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

                    outputWorkbook.SaveAs(_outputFilePath);
                }
            });
        }

        static async Task<string> GenerateOutputFileName(string _inputFilePath)
        {
            string result = string.Empty;
            await Task.Run(()=>
            {
                string directory = Path.GetDirectoryName(_inputFilePath) ?? ".";
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(_inputFilePath);
                result = Path.Combine(directory, $"{fileNameWithoutExt}_output.xlsx");
            });
            return result;
        }


    }

}
