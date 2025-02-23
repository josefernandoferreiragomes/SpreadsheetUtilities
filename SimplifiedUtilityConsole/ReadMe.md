# Simplified spreadsheet utility console

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
dotnet run .\..\ExampleFiles\input.xlsx 2 5 output.xlsx
```

## Walkthrough of the simplified project:

- Created the code for the console app,
- Then moved the logic to read the excel file and generate the double entry spreadsheet to a new library project.
- Added ClosedXML dependency:
	```bash
	Install-Package ClosedXML SimplifiedUtilityConsole
	```
- Tried Syncfusion.Blazor.Gantt, but it is paid, so I uninstalled it
- For the Gantt chart generator, added the following dependency:
	```bash	
	Install-Package
	```