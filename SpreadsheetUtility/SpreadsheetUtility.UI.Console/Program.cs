using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Utilities;
using Utilities.Interfaces;
using Utilities.Services;
// See https://aka.ms/new-console-template for more information
// Display help if arguments are missing

Console.WriteLine("Double Entry Spreadsheet Generator is about to start...");

if (args.Length < 1)
{
    Console.WriteLine("Usage: dotnet run <input.xlsx> keyColumnID valuesColumnID [output.xlsx] [headers row] [worksheet index]");
    return;
}

// Get input and output file paths
string keyColumnID = args.Length > 1 ? args[1] : string.Empty;
string valuesColumnID = args.Length > 2 ? args[2] : string.Empty;
string outputFilePath = args.Length > 3 ? args[3] : string.Empty;
string headersRow = args.Length > 4 ? args[4] : string.Empty;
string worksheetIndex = args.Length > 5 ? args[5] : string.Empty;

var host = CreateHostBuilder(args);
var service = host.Build().Services.GetRequiredService<IExcelWorkbook>();

//TODO: Use a factory to create the generator
SpreasheetGeneratorDoubleEntry generator = new SpreasheetGeneratorDoubleEntry(service, keyColumnID, valuesColumnID, outputFilePath, headersRow, worksheetIndex);

List<string> result = await generator.Generate();

foreach (var line in result)
{
    Console.WriteLine(line);
}

Console.WriteLine("Press any key to exit...");

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            string filePath = args[0];
            services.AddTransient<IExcelWorkbook>(factoryProvider => new ExcelWorkbook(filePath));
            services.AddTransient<SpreasheetGeneratorDoubleEntry>();
        });