using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Moq;
using Utilities;
using Utilities.Interfaces;
using Utilities.Services;
using Xunit;

namespace DoubleEntrySpreadsheetGeneratorTests
{

    public class DoubleEntrySpreasheetGeneratorIntegratedTests
    {
        //startup method
        

        [Fact]
        public async Task GenerateDoubleEntrySpreasheet_InputFileNotFound_ReturnsError()
        {            
            // Arrange
            var inputFilePath = "nonexistent.xlsx";
            var service = new ExcelWorkbook(inputFilePath);
            var generator = new DoubleEntrySpreasheetGenerator(service, "5", "3", "output.xlsx");

            // Act
            var result = await generator.GenerateDoubleEntrySpreasheet();

            // Assert
            Assert.Contains("Error: Input file 'nonexistent.xlsx' not found.", result);
        }

        [Fact]
        public async Task GenerateDoubleEntrySpreasheet_ValidInputFile_GeneratesOutputFile()
        {
            // Arrange
            var inputFilePath = "test_input.xlsx";
            var outputFilePath = "test_output.xlsx";
            CreateTestInputFile(inputFilePath);
            var service = new ExcelWorkbook(inputFilePath);

            var generator = new DoubleEntrySpreasheetGenerator(service, "2", "4", outputFilePath);

            // Act
            var result = await generator.GenerateDoubleEntrySpreasheet();

            // Assert
            Assert.Contains($"Output file saved: {outputFilePath}", result);
            Assert.True(File.Exists(outputFilePath));

            // Clean up
            File.Delete(inputFilePath);
            File.Delete(outputFilePath);
        }        

        [Fact]
        public async Task GenerateDoubleEntrySpreasheet_InputFileWithNoData_ReturnsError()
        {
            // Arrange
            var inputFilePath = "empty_input.xlsx";
            CreateEmptyTestInputFile(inputFilePath);
            var service = new ExcelWorkbook(inputFilePath);

            var generator = new DoubleEntrySpreasheetGenerator(service, "5", "3", "output.xlsx");

            // Act
            var result = await generator.GenerateDoubleEntrySpreasheet();

            // Assert
            Assert.Contains("Error: No valid data found in input file.", result);

            // Clean up
            File.Delete(inputFilePath);
        }

        private void CreateTestInputFile(string filePath)
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

        private void CreateEmptyTestInputFile(string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.Cell(1, 2).Value = "serviceName";
                worksheet.Cell(1, 4).Value = "thirdPartyServices";
                workbook.SaveAs(filePath);
            }
        }

        // TODO: after managing the excel reader as a dependency, add tests for the ReadInputFile method, using something like this:
        /*
         * var content = File.OpenRead(@"C:\myfile.xlsx");
        var file = new Mock<IFormFile>();
        file.Setup(_ => _.OpenReadStream()).Returns(content);
         */
    }
}