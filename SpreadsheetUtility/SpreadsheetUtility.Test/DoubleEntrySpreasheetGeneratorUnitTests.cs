﻿using ClosedXML.Excel;
using Moq;
using Utilities.Interfaces;

namespace Utilities.Test
{

    /// <summary>
    /// Still being developed...
    /// </summary>
    public class DoubleEntrySpreasheetGeneratorUnitTests
    {
        [Fact]
        public async Task GenerateDoubleEntrySpreasheet_InputFileNotFound_ReturnsError()
        {
            // Arrange
            var mockIXLWorkbook = new Mock<IXLWorkbook>();
            mockIXLWorkbook.Setup(wb => wb.Worksheet(It.IsAny<int>())).Returns(new Mock<IXLWorksheet>().Object);
            var mockWorkbook = new Mock<IExcelWorkbook>();
            
            var generator = new Mock<SpreasheetGeneratorDoubleEntry>();


            //generator.Setup(wb => wb.Generate()).Returns(Task.FromResult(new List<string>()
            //{
            //    "Error: Input file 'nonexistent.xlsx' not found."
            //}));
            //var generator = new DoubleEntrySpreasheetGenerator(mockWorkbook.Object, "1", "2", "output.xlsx");

            // Act
            var result = await generator.Object.Generate();

            // Assert
            Assert.Contains("Error: Input file 'nonexistent.xlsx' not found.", result);
        }

        [Fact]
        public async Task GenerateDoubleEntrySpreasheet_ValidInputFile_GeneratesOutputFile()
        {
            // Arrange
            var mockWorkbook = new Mock<IExcelWorkbook>();
            var mockWorksheet = new Mock<IXLWorksheet>();

            // Setup mock worksheet behavior
            mockWorksheet.Setup(ws => ws.LastRowUsed()).Returns(new Mock<IXLRow>().Object);
            mockWorksheet.Setup(ws => ws.Cell(It.IsAny<int>(), It.IsAny<string>()).GetString()).Returns("Test");

            // Setup mock workbook behavior
            mockWorkbook.Setup(wb => wb
                .Worksheet(It.IsAny<int>())
            ).Returns(mockWorksheet.Object);

            var generator = new SpreasheetGeneratorDoubleEntry(mockWorkbook.Object, "1", "2", "output.xlsx");

            // Act
            var result = await generator.Generate();

            // Assert
            Assert.Contains("Output file saved: output.xlsx", result);
        }

        [Fact]
        public async Task GenerateDoubleEntrySpreasheet_InputFileWithNoData_ReturnsError()
        {
            // Arrange
            var mockWorkbook = new Mock<IExcelWorkbook>();
            var mockWorksheet = new Mock<IXLWorksheet>();

            // Setup mock worksheet behavior
            mockWorksheet.Setup(ws => ws.LastRowUsed()).Returns(null as IXLRow);

            // Setup mock workbook behavior
            mockWorkbook.Setup(wb => wb.Worksheet(It.IsAny<int>())).Returns(mockWorksheet.Object);

            var generator = new SpreasheetGeneratorDoubleEntry(mockWorkbook.Object, "1", "2", "output.xlsx");

            // Act
            var result = await generator.Generate();

            // Assert
            Assert.Contains("Error: Input file does not contain data.", result);
        }
    }

}
