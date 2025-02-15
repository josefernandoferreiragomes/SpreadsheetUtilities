using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;

namespace Utilities
{
    public class DoubleEntrySpreasheetGenerator
    {
        private string _inputFilePath;
        private string _outputFilePath;

        public DoubleEntrySpreasheetGenerator(string inputFilePath, string outputFilePath)
        {
            _inputFilePath = inputFilePath;
            _outputFilePath = outputFilePath;
        }

        public async Task<List<string>> GenerateDoubleEntrySpreasheet()                 
        {    
            Lazy<List<string>> result = new Lazy<List<string>>();            
            _outputFilePath = string.IsNullOrEmpty(_outputFilePath) ? GenerateOutputFileName(_inputFilePath) : _outputFilePath;

            if (!File.Exists(_inputFilePath))
            {
                result.Value.Add($"Error: Input file '{_inputFilePath}' not found.");
                return result.Value;
            }

            var serviceMappings = new Dictionary<string, HashSet<string>>();
            var uniqueThirdPartyServices = new HashSet<string>();

            try
            {
                using (var workbook = new XLWorkbook(_inputFilePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    int rowCount = worksheet.LastRowUsed()?.RowNumber() ?? 0;

                    if (rowCount < 2)
                    {
                        result.Value.Add("Error: Input file does not contain data.");
                        return result.Value;
                    }

                    for (int row = 2; row <= rowCount; row++) // Skip header row
                    {
                        string serviceName = worksheet.Cell(row, 2).GetString().Trim(); // Column B: serviceName
                        string thirdPartyCell = worksheet.Cell(row, 4).GetString().Trim(); // Column D: third party services

                        if (string.IsNullOrEmpty(serviceName)) continue; // Skip empty service names

                        if (!serviceMappings.ContainsKey(serviceName))
                            serviceMappings[serviceName] = new HashSet<string>();

                        foreach (var thirdPartyService in thirdPartyCell.Split(new[] { '\n', ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string service = thirdPartyService.Trim();
                            if (!string.IsNullOrEmpty(service))
                            {
                                serviceMappings[serviceName].Add(service);
                                uniqueThirdPartyServices.Add(service);
                            }
                        }
                    }
                }

                if (serviceMappings.Count == 0)
                {
                    result.Value.Add("Error: No valid data found in input file.");
                    return result.Value;
                }

                // Generate the output table
                using (var outputWorkbook = new XLWorkbook())
                {
                    var outputWorksheet = outputWorkbook.Worksheets.Add("Service Mapping");

                    // Write headers
                    outputWorksheet.Cell(1, 1).Value = "serviceName";
                    int colIndex = 2;
                    var serviceColumns = uniqueThirdPartyServices.OrderBy(s => s).ToList();

                    foreach (var service in serviceColumns)
                    {
                        outputWorksheet.Cell(1, colIndex++).Value = service;
                    }

                    // Fill data
                    int rowIndex = 2;
                    foreach (var entry in serviceMappings)
                    {
                        outputWorksheet.Cell(rowIndex, 1).Value = entry.Key; // Service name
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

                result.Value.Add($"Output file saved: {_outputFilePath}");
            }
            catch (Exception ex)
            {
                result.Value.Add($"Error processing the file: {ex.Message}");
            }
            return result.Value;
        }

        static string GenerateOutputFileName(string _inputFilePath)
        {
            string directory = Path.GetDirectoryName(_inputFilePath) ?? ".";
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(_inputFilePath);
            return Path.Combine(directory, $"{fileNameWithoutExt}_output.xlsx");
        }
    }

}
