using MediatR;
using SpreadsheetUtility.Application.DTOs;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Application.UseCases.GenerateDoubleEntrySpreadsheet;

public class GenerateDoubleEntrySpreadsheetHandler : IRequestHandler<GenerateDoubleEntrySpreadsheetCommand, GenerateDoubleEntryOutput>
{
    private readonly IDoubleEntryGeneratorService _generator;

    public GenerateDoubleEntrySpreadsheetHandler(IDoubleEntryGeneratorService generator)
    {
        _generator = generator;
    }

    public async Task<GenerateDoubleEntryOutput> Handle(GenerateDoubleEntrySpreadsheetCommand request, CancellationToken cancellationToken)
    {
        var input = new GenerateDoubleEntryInput
        {
            InputFilePath = request.InputFilePath,
            KeyColumnId = request.KeyColumnId,
            ValuesColumnId = request.ValuesColumnId,
            OutputFilePath = request.OutputFilePath,
            HeadersRow = request.HeadersRow,
            WorksheetIndex = request.WorksheetIndex
        };

        return await _generator.GenerateAsync(input, cancellationToken);
    }
}
