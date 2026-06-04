using Moq;
using SpreadsheetUtility.Application.DTOs;
using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Application.UseCases.GenerateDoubleEntrySpreadsheet;

namespace SpreadsheetUtility.Test.ApplicationTests.UseCases;

public class GenerateDoubleEntrySpreadsheetHandlerTests
{
    private readonly Mock<IDoubleEntryGeneratorService> _generatorMock;
    private readonly GenerateDoubleEntrySpreadsheetHandler _handler;

    public GenerateDoubleEntrySpreadsheetHandlerTests()
    {
        _generatorMock = new Mock<IDoubleEntryGeneratorService>();
        _handler = new GenerateDoubleEntrySpreadsheetHandler(_generatorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Call_Generator_And_Return_Result()
    {
        var command = new GenerateDoubleEntrySpreadsheetCommand
        {
            InputFilePath = "input.xlsx",
            KeyColumnId = "1",
            ValuesColumnId = "2",
            OutputFilePath = "output.xlsx"
        };
        var expectedOutput = new GenerateDoubleEntryOutput { Success = true };
        _generatorMock.Setup(g => g.GenerateAsync(It.IsAny<GenerateDoubleEntryInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedOutput);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Success);
        _generatorMock.Verify(g => g.GenerateAsync(
            It.Is<GenerateDoubleEntryInput>(i =>
                i.InputFilePath == "input.xlsx" &&
                i.KeyColumnId == "1" &&
                i.ValuesColumnId == "2" &&
                i.OutputFilePath == "output.xlsx"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Map_All_Command_Properties()
    {
        var command = new GenerateDoubleEntrySpreadsheetCommand
        {
            InputFilePath = "in.xlsx",
            KeyColumnId = "3",
            ValuesColumnId = "4",
            OutputFilePath = "out.xlsx",
            HeadersRow = "1",
            WorksheetIndex = "0"
        };
        _generatorMock.Setup(g => g.GenerateAsync(It.IsAny<GenerateDoubleEntryInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GenerateDoubleEntryOutput { Success = true });

        await _handler.Handle(command, CancellationToken.None);

        _generatorMock.Verify(g => g.GenerateAsync(
            It.Is<GenerateDoubleEntryInput>(i =>
                i.HeadersRow == "1" &&
                i.WorksheetIndex == "0"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
