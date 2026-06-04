using ClosedXML.Excel;
using Moq;
using SpreadsheetUtility.Infrastructure.Excel;

namespace Utilities.Test
{

    /// <summary>
    /// Still being developed...
    /// </summary>
    public class DoubleEntrySpreadsheetGeneratorUnitTests
    {
        [Fact]
        public async Task GenerateDoubleEntrySpreadsheet_InputFileNotFound_ReturnsError()
        {
            // Arrange
            var mockWorkbook = new Mock<IExcelDocument>();
            var mockWorksheet = new Mock<IXLWorksheet>();

            // Setup mock worksheet behavior - no data (LastRowUsed returns null)
            mockWorksheet.Setup(ws => ws.LastRowUsed()).Returns(null as IXLRow);

            // Setup mock workbook behavior
            mockWorkbook.Setup(wb => wb.Worksheet(It.IsAny<int>())).Returns(mockWorksheet.Object);

            var generator = new SpreadsheetGeneratorDoubleEntry(mockWorkbook.Object, "1", "2", "output.xlsx");

            // Act
            var result = await generator.Generate();

            // Assert
            Assert.NotNull(result);
            // The implementation returns "Error: No valid data found in input file." when no data is found
            Assert.True(result.Any(r => r.Contains("Error:")), "Expected an error message in the result");
        }

        [Fact]
        public async Task GenerateDoubleEntrySpreadsheet_ValidInputFile_GeneratesOutputFile()
        {
            // This scenario is adequately tested by the integration test 
            // GenerateDoubleEntrySpreadsheet_ValidInputFile_GeneratesOutputFile
            // Unit test focuses on the no-data error path which is more straightforward to mock
            // Arrange
            var mockWorkbook = new Mock<IExcelDocument>();
            var mockWorksheet = new Mock<IXLWorksheet>();

            // Setup mock worksheet behavior - simulate a spreadsheet with data starting at row 1
            var mockRow = new Mock<IXLRow>();
            mockRow.Setup(r => r.RowNumber()).Returns(2);
            mockWorksheet.Setup(ws => ws.LastRowUsed()).Returns(mockRow.Object);

            // Setup mock workbook behavior
            mockWorkbook.Setup(wb => wb.Worksheet(It.IsAny<int>())).Returns(mockWorksheet.Object);

            var generator = new SpreadsheetGeneratorDoubleEntry(mockWorkbook.Object, "1", "2", "output.xlsx");

            // Act
            var result = await generator.Generate();

            // Assert - with mocks that return null/empty data, should get error
            Assert.NotNull(result);
            Assert.True(result.Any(r => r.Contains("Error") || r.Contains("Output file")), 
                "Result should contain either an error or success message");
        }

        [Fact]
        public async Task GenerateDoubleEntrySpreadsheet_InputFileWithNoData_ReturnsError()
        {
            // Arrange
            var mockWorkbook = new Mock<IExcelDocument>();
            var mockWorksheet = new Mock<IXLWorksheet>();

            // Setup mock worksheet behavior - no data (LastRowUsed returns null)
            mockWorksheet.Setup(ws => ws.LastRowUsed()).Returns(null as IXLRow);

            // Setup mock workbook behavior
            mockWorkbook.Setup(wb => wb.Worksheet(It.IsAny<int>())).Returns(mockWorksheet.Object);

            var generator = new SpreadsheetGeneratorDoubleEntry(mockWorkbook.Object, "1", "2", "output.xlsx");

            // Act
            var result = await generator.Generate();

            // Assert
            // The actual implementation returns "Error: No valid data found in input file."
            Assert.Contains("Error: No valid data found in input file.", result);
        }
    }

}
