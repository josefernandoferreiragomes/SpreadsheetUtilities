using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using ClosedXML.Excel;
using Utilities.Interfaces;

namespace Utilities
{
    public class SpreasheetGeneratorMultiRow : SpreasheetGeneratorBase
    {

        public SpreasheetGeneratorMultiRow(IExcelWorkbook workbook, string keyColumnID, string valuesColumnID, string outputFilePath, string? headersRow = null, string? worksheetIndex = null)
            : base(workbook, keyColumnID, valuesColumnID, outputFilePath, headersRow, worksheetIndex)
        {
        }

        public override async Task<List<string>> Generate()
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

                var uniqueThirdPartyServices = GetSplitCellRows(serviceMappings);
                await WriteOutputFile(serviceMappings, uniqueThirdPartyServices).ConfigureAwait(false);

                result.Add($"Output file saved: {_outputFilePath}");
            }
            catch (Exception ex)
            {
                result.Add($"Error processing the file: {ex.Message}");
            }

            return result;
        }

        private async Task<Dictionary<string, HashSet<string>>> ReadInputFile()
        {            
            var keyMappings = new Dictionary<string, HashSet<string>>();
            await Task.Run(() =>
            {  
                                 
                var worksheet = _workbook.Worksheet(_worksheetIndexInt);
                int rowCount = worksheet.LastRowUsed()?.RowNumber() ?? 0;


                for (int row = _headersRowInt; row <= rowCount; row++)
                {
                    string keyColumn = worksheet.Cell(row, _keyColumnIDint.ToString()).GetString().Trim();
                    string valuesColumn = worksheet.Cell(row, _valuesColumnIDint.ToString()).GetString().Trim();

                    if (string.IsNullOrEmpty(keyColumn)) continue;

                    if (!keyMappings.ContainsKey(keyColumn))
                        keyMappings[keyColumn] = new HashSet<string>();

                    foreach (var thirdPartyService in valuesColumn.Split(new[] { '\n', ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string service = thirdPartyService.Trim();
                        if (!string.IsNullOrEmpty(service))
                        {
                            keyMappings[keyColumn].Add(service);
                        }
                    }
                }                          
            });            
            return keyMappings;
        }      

        protected override async Task WriteOutputFile(Dictionary<string, HashSet<string>> serviceMappings, HashSet<string> uniqueThirdPartyServices)
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
