using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpreadsheetUtility.Application.UseCases.GenerateDoubleEntrySpreadsheet;
using SpreadsheetUtility.Bootstrapper;

Console.WriteLine("Double Entry Spreadsheet Generator is about to Start...");

if (args.Length < 1)
{
    Console.WriteLine("Usage: dotnet run <input.xlsx> keyColumnID valuesColumnID [output.xlsx] [headers row] [worksheet index]");
    return;
}

var host = CreateHostBuilder(args).Build();
var mediator = host.Services.GetRequiredService<IMediator>();

var command = new GenerateDoubleEntrySpreadsheetCommand
{
    InputFilePath = args[0],
    KeyColumnId = args.Length > 1 ? args[1] : string.Empty,
    ValuesColumnId = args.Length > 2 ? args[2] : string.Empty,
    OutputFilePath = args.Length > 3 ? args[3] : string.Empty,
    HeadersRow = args.Length > 4 ? args[4] : string.Empty,
    WorksheetIndex = args.Length > 5 ? args[5] : string.Empty
};

var result = await mediator.Send(command);

foreach (var line in result.Messages)
{
    Console.WriteLine(line);
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            services.AddSpreadsheetUtilities();
        });
