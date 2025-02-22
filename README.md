# Double Entry Spreadsheet Generator

This console application generates double entry spreadsheets from single entry spreadsheets with multi line cells.

For example, if You have a spreadsheet with the following data:

|Id|CarModel|LicensePlate|ManufacturingYear|Features|
|---|---|---|---|---|
|1|CarModelA|LicensePlate1|1985|AirConditioning<br>PowerSteering|
|2|CarModelB|LicensePlate2|1995|PowerSteering<br>BucketSeats|
|3|CarModelC|LicensePlate3|1992|AirConditioning<br>BucketSeats|

You can convert it to a double entry spreadsheet with the following data:

|Id|CarModel|AirConditioning|PowerSteering|BucketSeats|
|---|---|---|---|---|
|1|CarModelA|X|X|
|1|CarModelB||X|X|
|1|CarModelC|X||X|


## Prerequisites

- .NET 8 SDK
- A single entry spreadsheet file (e.g., `input.xlsx`)

## Usage

### From visual studio Developer Powershell / Terminal

1. Clone the repository or download the source code.
2. Open a terminal and navigate to the project directory.
3. Run the application using the following command:

```bash
dotnet run <input.xlsx> <key column> <values column> [output.xlsx] [headers row] [worksheet index]
```

   - `<input.xlsx>`   : The path to the input single entry spreadsheet file.
   - `<key column>`   : The column index (1-based) which contains the keys.
   - `<values column>`: The column index (1-based) which contains the values.
   - `[output.xlsx]` (optional): The path to the output double entry spreadsheet file. If not provided, the output will be saved in the same directory as the input file with a default name.
   - `[headers row]` (optional): The row which contains the table headers.
   - `[worksheet index]` (optional): The row which contains the table headers.

### Executing the application as portable

You may also publish the console application and use it as portable, 

In the following example, it is run from CMD from the exe file folder, considering the input file is in the same directory:
```bash
.\SpreadsheetUtilityConsole.exe input.xlsx 2 5 output.xlsx
```

## Example

Developer PowerShell, from the project folder:
```bash
dotnet run .\input.xlsx 2 5 output.xlsx
```

This command will generate a double entry spreadsheet from `input.xlsx` and save it as `output.xlsx`.

## Notes

- If the output file path is not specified, the application will generate the output file in the same directory as the input file with a default name.
- Ensure that the input file exists and is accessible.
- The application assumes that the input file is a valid Excel file with the `.xlsx` extension.
- If the output file already exists, it will be overwritten.
- If the headers row is not provided, the application will use the first row as the headers.
- If the worksheet index is not provided, the application will use the first worksheet.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE.txt) file for details.

## Walkthrough

Created the code for the console app, 

Then moved the logic to read the excel file and generate the double entry spreadsheet to a new library project.

Refactored the previous code.

Adapted the projects to use the same solution and added the tests for the logic.

Created the code for the tests.

In Package Manager Console:

Added the moq library to the test project to mock the excel file reading:
```bash
Install-Package moq
```

Updated the xunit package:
```bash
Update-Package xunit
```

Added the Microsoft.Extensions.Hosting and Microsoft.Extensions.Hosting.Abstractions to the library project:
```bash
Install-Package Microsoft.Extensions.Hosting
Install-Package Microsoft.Extensions.Hosting.Abstractions
```

## Next features
Multi line cells to multi rows
One entry tables with single line cells to Double entry



