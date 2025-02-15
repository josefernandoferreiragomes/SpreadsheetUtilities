using Utilities;
// See https://aka.ms/new-console-template for more information
// Display help if arguments are missing

Console.WriteLine("Double Entry Spreadsheet Generator is about to start...");



if (args.Length < 1)
{
    Console.WriteLine("Usage: dotnet run <input.xlsx> [output.xlsx]");
    return;
}

// Get input and output file paths
string inputFilePath = args[0];
string outputFilePath = args.Length > 1 ? args[1] : string.Empty;

DoubleEntrySpreasheetGenerator generator = new DoubleEntrySpreasheetGenerator(inputFilePath, outputFilePath);
List<string> result = await generator.GenerateDoubleEntrySpreasheet();

foreach (var line in result)
{
    Console.WriteLine(line);
}

Console.WriteLine("Press any key to exit...");

