using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using SpreadsheetUtility.Application.DTOs;
using SpreadsheetUtility.Infrastructure.Excel;
using Xunit;

namespace SpreadsheetUtility.Test.InfrastructureTests;

[Trait("Category", "Integration")]
public class DoubleEntrySpreadsheetGeneratorIntegratedTests
{
    [Fact]
    public async Task GenerateDoubleEntrySpreadsheet_InputFileNotFound_ReturnsError()
    {
        // Arrange
        var inputFilePath = "nonexistent.xlsx";
        var service = new DoubleEntryGeneratorService();
        var input = new GenerateDoubleEntryInput
        {
            InputFilePath = inputFilePath,
            KeyColumnId = "5",
            ValuesColumnId = "3",
            OutputFilePath = "output.xlsx"
        };

        // Act
        var result = await service.GenerateAsync(input);

        // Assert
        Assert.False(result.Success);
        Assert.Contains($"Error: Input file '{inputFilePath}' not found.", result.Messages);
    }

    [Fact]
    public async Task GenerateDoubleEntrySpreadsheet_ValidInputFile_GeneratesOutputFile()
    {
        // Arrange
        var inputFilePath = "test_input.xlsx";
        var outputFilePath = "test_output.xlsx";
        CreateTestInputFile(inputFilePath);
        var service = new DoubleEntryGeneratorService();
        var input = new GenerateDoubleEntryInput
        {
            InputFilePath = inputFilePath,
            KeyColumnId = "2",
            ValuesColumnId = "4",
            OutputFilePath = outputFilePath
        };

        // Act
        var result = await service.GenerateAsync(input);

        // Assert
        Assert.True(result.Success);
        Assert.Contains($"Output file saved: {outputFilePath}", result.Messages);
        Assert.True(File.Exists(outputFilePath));

        // Clean up
        File.Delete(inputFilePath);
        File.Delete(outputFilePath);
    }

    [Fact]
    public async Task GenerateDoubleEntrySpreadsheet_InputFileWithNoData_ReturnsError()
    {
        // Arrange
        var inputFilePath = "empty_input.xlsx";
        CreateEmptyTestInputFile(inputFilePath);
        var service = new DoubleEntryGeneratorService();
        var input = new GenerateDoubleEntryInput
        {
            InputFilePath = inputFilePath,
            KeyColumnId = "5",
            ValuesColumnId = "3",
            OutputFilePath = "output.xlsx"
        };

        // Act
        var result = await service.GenerateAsync(input);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Error: No valid data found in input file.", result.Messages);

        // Clean up
        File.Delete(inputFilePath);
    }

    private static void CreateTestInputFile(string filePath)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Sheet1");
            worksheet.Cell(1, 2).Value = "serviceName";
            worksheet.Cell(1, 4).Value = "thirdPartyServices";
            worksheet.Cell(2, 2).Value = "Service1";
            worksheet.Cell(2, 4).Value = "ThirdParty1, ThirdParty2";
            workbook.SaveAs(filePath);
        }
    }

    private static void CreateEmptyTestInputFile(string filePath)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Sheet1");
            worksheet.Cell(1, 2).Value = "serviceName";
            worksheet.Cell(1, 4).Value = "thirdPartyServices";
            workbook.SaveAs(filePath);
        }
    }
}

