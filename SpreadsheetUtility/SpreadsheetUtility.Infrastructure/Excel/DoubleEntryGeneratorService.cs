using SpreadsheetUtility.Application.DTOs;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Infrastructure.Excel;

public class DoubleEntryGeneratorService : IDoubleEntryGeneratorService
{
    private const int WORKSHEET_INDEX_DEFAULT = 1;
    private const int HEADER_ROW_INDEX_DEFAULT = 2;

    public async Task<GenerateDoubleEntryOutput> GenerateAsync(GenerateDoubleEntryInput input, CancellationToken cancellationToken = default)
    {
        var result = new GenerateDoubleEntryOutput();
        var messages = new List<string>();

        if (string.IsNullOrEmpty(input.KeyColumnId) || !int.TryParse(input.KeyColumnId, out var keyCol))
        {
            messages.Add("Error: Key Column ID is missing or invalid.");
            result.Messages = messages;
            return result;
        }

        if (string.IsNullOrEmpty(input.ValuesColumnId) || !int.TryParse(input.ValuesColumnId, out var valCol))
        {
            messages.Add("Error: Values Column ID is missing or invalid.");
            result.Messages = messages;
            return result;
        }

        int headerRow = int.TryParse(input.HeadersRow, out var h) ? h : HEADER_ROW_INDEX_DEFAULT;
        int worksheetIdx = int.TryParse(input.WorksheetIndex, out var w) ? w : WORKSHEET_INDEX_DEFAULT;

        try
        {
            var doc = new ExcelDocument(input.InputFilePath);

            string outputPath = string.IsNullOrEmpty(input.OutputFilePath)
                ? await doc.GenerateOutputFileName()
                : input.OutputFilePath;

            var mappings = await ReadInputFile(doc, keyCol, valCol, headerRow, worksheetIdx);

            if (mappings.Count == 0)
            {
                messages.Add("Error: No valid data found in input file.");
                result.Messages = messages;
                return result;
            }

            var uniqueServices = GetUniqueServices(mappings);
            WriteOutputFile(doc, mappings, uniqueServices, outputPath);

            result.Success = true;
            messages.Add($"Output file saved: {outputPath}");
            result.OutputFilePath = outputPath;
        }
        catch (FileNotFoundException ex)
        {
            messages.Add($"Error: Input file '{input.InputFilePath}' not found.");
        }
        catch (Exception ex)
        {
            messages.Add($"Error processing the file: {ex.Message}");
        }

        result.Messages = messages;
        return result;
    }

    private static async Task<Dictionary<string, HashSet<string>>> ReadInputFile(
        IExcelDocument doc, int keyCol, int valCol, int headerRow, int worksheetIdx)
    {
        var mappings = new Dictionary<string, HashSet<string>>();
        await Task.Run(() =>
        {
            var worksheet = doc.Worksheet(worksheetIdx);
            int rowCount = worksheet.LastRowUsed()?.RowNumber() ?? 0;

            for (int row = headerRow; row <= rowCount; row++)
            {
                string key = worksheet.Cell(row, keyCol.ToString()).GetString().Trim();
                string values = worksheet.Cell(row, valCol.ToString()).GetString().Trim();

                if (string.IsNullOrEmpty(key)) continue;

                if (!mappings.ContainsKey(key))
                    mappings[key] = new HashSet<string>();

                foreach (var item in values.Split(new[] { '\n', ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string val = item.Trim();
                    if (!string.IsNullOrEmpty(val))
                        mappings[key].Add(val);
                }
            }
        });
        return mappings;
    }

    private static HashSet<string> GetUniqueServices(Dictionary<string, HashSet<string>> mappings)
    {
        var unique = new HashSet<string>();
        foreach (var entry in mappings)
        {
            foreach (var service in entry.Value)
            {
                unique.Add(service);
            }
        }
        return unique;
    }

    private static void WriteOutputFile(
        IExcelDocument doc,
        Dictionary<string, HashSet<string>> mappings,
        HashSet<string> uniqueServices,
        string outputPath)
    {
        var outputWorksheet = doc.AddWorksheet("Service Mapping");

        outputWorksheet.Cell(1, 1).Value = "keyColumn";
        int colIndex = 2;
        var serviceColumns = uniqueServices.OrderBy(s => s).ToList();

        foreach (var service in serviceColumns)
        {
            outputWorksheet.Cell(1, colIndex++).Value = service;
        }

        int rowIndex = 2;
        foreach (var entry in mappings)
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

        doc.SaveAs(outputPath);
    }
}
