using SpreadsheetUtility.Application.DTOs;
using SpreadsheetUtility.Infrastructure.Excel;

namespace SpreadsheetUtility.Test;

public class DoubleEntrySpreadsheetGeneratorUnitTests
{
    [Fact]
    public async Task GenerateDoubleEntry_InvalidKeyColumnId_ReturnsError()
    {
        var service = new DoubleEntryGeneratorService();
        var input = new GenerateDoubleEntryInput
        {
            InputFilePath = "dummy.xlsx",
            KeyColumnId = "",
            ValuesColumnId = "3",
            OutputFilePath = "output.xlsx"
        };

        var result = await service.GenerateAsync(input);

        Assert.False(result.Success);
        Assert.Contains("Error: Key Column ID is missing or invalid.", result.Messages);
    }

    [Fact]
    public async Task GenerateDoubleEntry_InvalidValuesColumnId_ReturnsError()
    {
        var service = new DoubleEntryGeneratorService();
        var input = new GenerateDoubleEntryInput
        {
            InputFilePath = "dummy.xlsx",
            KeyColumnId = "1",
            ValuesColumnId = "abc",
            OutputFilePath = "output.xlsx"
        };

        var result = await service.GenerateAsync(input);

        Assert.False(result.Success);
        Assert.Contains("Error: Values Column ID is missing or invalid.", result.Messages);
    }

    [Fact]
    public async Task GenerateDoubleEntry_ValidInput_NoFileExistsReturnsError()
    {
        var inputFilePath = "nonexistent_file.xlsx";
        var service = new DoubleEntryGeneratorService();
        var input = new GenerateDoubleEntryInput
        {
            InputFilePath = inputFilePath,
            KeyColumnId = "5",
            ValuesColumnId = "3",
            OutputFilePath = "output.xlsx"
        };

        var result = await service.GenerateAsync(input);

        Assert.False(result.Success);
        Assert.Contains($"Error: Input file '{inputFilePath}' not found.", result.Messages);
    }
}
