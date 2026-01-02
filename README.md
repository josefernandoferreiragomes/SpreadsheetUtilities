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
Blazor Gantt Generator
Multi line cells to multi rows
One entry tables with single line cells to Double entry

# Blazor Gantt Chart Application

## Overview
This application is a **Blazor WebAssembly** project that generates a **Gantt Chart** based on task assignments from an Excel file. It uses **BlazorGantt** for visualization and **ClosedXML** for reading Excel spreadsheets.

## Features
- Reads task and team data from two Excel files:
  - **Tasks.xlsx**: Contains project tasks with estimated effort hours.
  - **Team.xlsx**: Contains developers, their daily work hours, and vacation periods.
- Assigns tasks to developers while considering vacation periods.
- Generates a Gantt Chart to visualize task assignments.

## Required Packages
Install the following NuGet packages:
```sh
# ClosedXML for Excel processing
dotnet add package ClosedXML
````
# Gantt chart rendering, in the Blazor app
Frappe Gantt:
https://github.com/frappe/gantt
https://frappe.io/gantt

```bash
dotnet add package frappe-gantt
dotnet add package Microsoft.JSInterop 
```
# To allow for Gantt chart rendering, in the Library
```bash
dotnet add package newtonsoft.json
```


## File Structure
- **GanttChartService.cs**: Processes Excel data, assigns tasks, and handles scheduling logic.
- **Gantt.razor**: Blazor page that renders the Gantt Chart using BlazorGantt.
- **wwwroot/**: Place your **Tasks.xlsx** and **Team.xlsx** files here.

## License
This project uses the following licenses:
- **ClosedXML**: MIT License (Free for personal and commercial use)
- **BlazorGantt**: MIT License (Free and open-source)

## How to Run
1. Place `Tasks.xlsx` and `Team.xlsx` in the `wwwroot/` folder.
2. Run the Blazor app:
   ```sh
   dotnet run
   ```
3. Open your browser and navigate to `/gantt` to see the Gantt Chart.

---
For any questions or improvements, feel free to contribute!


## Requirements for paste from excel:
- Paste areas in a page
- Generate button

- Add Quick Grid to the project
```bash
dotnet add package Microsoft.AspNetCore.Components.QuickGrid
```

- Add project pre sorting
- Add task end week

- Add project groupping, by projects allowed to be assigned togehter, in batches

---

# Architecture & Design Patterns

The **SpreadsheetUtility.Library** project implements several industry-standard design patterns to promote maintainability, testability, and extensibility. This section documents each pattern and its benefits.

## 1. Observer Pattern

**Location:** `DateCalculator.cs`, `GanttChartProcessor.cs`

Implements notification mechanism for holiday detection events.

**Key Components:**
- `IObserver<Holiday>` interface (from System namespace)
- `AddObserver()` and `RemoveObserver()` methods in `DateCalculator`
- `NotifyObservers()` for publishing holiday events
- `GanttChartProcessor` implements `IObserver<Holiday>` to handle notifications

**Advantages:**
- Decouples holiday detection logic from holiday handling
- Multiple observers can react to holidays independently
- Enables reactive programming without tight coupling
- Easy to add new observers without modifying existing code

---

## 2. Strategy Pattern

**Location:** Task assignment and task sorting strategies

Enables runtime switching between different task assignment and sorting algorithms.

**Key Components:**
- `ITaskAssignmentStrategy` with `DefaultTaskAssignmentStrategy`
- `ITaskSortingStrategy` with `DefaultTaskSortingStrategy` and `TaskSortingStrategyEffortBased`
- Strategy base classes provide common functionality

**Advantages:**
- Algorithms can be swapped at runtime without changing client code
- New strategies can be added without modifying existing implementations
- Each strategy is independently testable
- Follows Open/Closed Principle (open for extension, closed for modification)

---

## 3. Factory Pattern

**Location:** Strategy factories

Centralizes strategy instantiation and integrates with dependency injection.

**Key Components:**
- `ITaskAssignmentStrategyFactory` / `TaskAssignmentStrategyFactory`
- `ITaskSortingStrategyFactory` / `TaskSortingStrategyFactory`
- Both factories use `IServiceProvider` for strategy resolution

**Advantages:**
- Decouples strategy instantiation from usage
- Centralizes creation logic for easier maintenance
- Leverages dependency injection for lifecycle management
- Simplifies testing by allowing factory substitution

---

## 4. Template Method Pattern

**Location:** `TaskAssignmentStrategyBase.cs`, `ListGenerator<TInput, TOutput>`

Defines skeleton algorithms in base classes while letting subclasses implement specific steps.

**Key Components:**
- Base classes define common algorithm structure
- Protected abstract methods define customization points
- Subclasses implement specific behavior while reusing common logic

**Advantages:**
- Promotes code reuse across implementations
- Provides consistent structure for similar operations
- Reduces code duplication
- Makes the algorithm structure explicit and maintainable

---

## 5. Builder Pattern

**Location:** `CalculateGanttChartAllocationOutputBuilder.cs`

Simplifies construction of complex objects with many parameters.

**Key Components:**
- Fluent API with method chaining
- Methods like `WithProjects()`, `WithGanttTasks()`, etc.
- `Build()` method returns the final constructed object

**Advantages:**
- Improves code readability with descriptive method names
- Handles complex object initialization elegantly
- Immutable result object ensures consistency
- Optional parameters are clearly expressed

---

## 6. Facade Pattern

**Location:** `CalculatorFacade.cs`, `ICalculatorFacade.cs`

Provides unified interface to multiple calculator services.

**Key Components:**
- `ICalculatorFacade` interface with properties for different calculators
- `DateCalculator` and `DeveloperHoursCalculator` exposed through facade
- Simplifies client interaction with multiple services

**Advantages:**
- Reduces complexity of working with multiple services
- Single entry point for calculator operations
- Easy to extend with new calculator services
- Decouples clients from implementation details

---

## 7. Mapper/Adapter Pattern

**Location:** `GanttChartMapper.cs`, `IGanttChartMapper.cs`

Transforms between different data representations (DTOs and domain models).

**Key Components:**
- `MapGanttTasksFromTaskDtos()` - TaskDto → GanttTask
- `MapProjectsFromProjectDtos()` - ProjectDto → Project
- `MapDevelopersFromDeveloperDtos()` - DeveloperDto → Developer
- `MapDeveloperAvailabilitiesFromDevelopers()` - Developer → DeveloperAvailability

**Advantages:**
- Decouples DTOs from domain models
- Enables clean transformation between data representations
- Centralizes mapping logic for easier maintenance
- Facilitates testing by separating transformation concerns

---

## 8. Generic List Generator Pattern

**Location:** `IListGenerator<TInput, TOutput>` and implementations

Provides type-safe, reusable grouping and aggregation logic for different data types.

**Key Components:**
- `IListGenerator<TInput, TOutput>` generic interface
- `GanttTaskProjectListGenerator` - Groups GanttTask → Project
- `GanttTaskListGenerator` - Groups GanttTask → GanttTask
- `DeveloperTaskListGenerator` - Groups Developer → List<GanttTask>

**Advantages:**
- Type safety with generic constraints
- Reduces code duplication across similar aggregation scenarios
- Each generator is independently testable
- Easy to add new generators for new data types

---

## 9. Command Pattern

**Location:** `ILogCommand.cs`, logging command implementations

Encapsulates logging requests as objects for deferred execution.

**Key Components:**
- `ILogCommand` interface with `Execute()` method
- `ProcessorMessageLogCommand` - Logs processor messages
- `CalculateGanttChartAllocationInputLogCommand` - Logs allocation input
- `CalculateGanttChartAllocationOutputLogCommand` - Logs allocation output
- `LoggingInvoker` manages command queue and execution

**Advantages:**
- Encapsulates logging requests as first-class objects
- Enables batching multiple logging operations
- Supports deferred execution of logging commands
- Separates logging request creation from execution
- Easy to add new logging commands without modifying invoker

---

## 10. Dependency Injection Pattern

**Location:** Throughout the library and Program.cs

Promotes loose coupling and testability through constructor injection.

**Key Components:**
- Service registration in `Program.cs`
- Constructor injection in all service classes
- IServiceProvider for runtime service resolution

**Advantages:**
- Improves testability by enabling dependency mocking
- Reduces tight coupling between components
- Centralizes dependency configuration
- Enables easy service substitution for different scenarios
- Supports lifecycle management (Scoped, Transient, Singleton)

---

## 11. Provider Pattern

**Location:** `IHolidayProvider.cs`, `IDateTimeProvider.cs`, and implementations

Abstracts external dependencies for better testability and flexibility.

**Key Components:**
- `IHolidayProvider` - Provides holiday data from configuration
- `IDateTimeProvider` - Abstracts date/time access
- Implementations handle actual data retrieval

**Advantages:**
- Abstracts external system dependencies
- Enables easy substitution for testing with mock providers
- Isolates business logic from infrastructure concerns
- Supports multiple provider implementations

---

## Pattern Summary Table

| Pattern | Location | Primary Benefit |
|---------|----------|-----------------|
| **Observer** | DateCalculator, GanttChartProcessor | Event notification decoupling |
| **Strategy** | Task assigners/sorters | Runtime algorithm switching |
| **Factory** | Strategy factories | Centralized creation with DI |
| **Template Method** | Base strategy/generator classes | Code reuse, consistent structure |
| **Builder** | CalculateGanttChartAllocationOutputBuilder | Complex object construction |
| **Facade** | CalculatorFacade | Unified multi-service interface |
| **Mapper** | GanttChartMapper | DTO ↔ Domain model transformation |
| **Generic Generators** | ListGenerator<TInput, TOutput> | Type-safe reusable grouping |
| **Command** | LoggingInvoker + ILogCommand | Encapsulated, deferred logging |
| **Dependency Injection** | Throughout library | Testability, loose coupling |
| **Provider** | Holiday/DateTime providers | External dependency abstraction |

---

## Design Principles Applied

The codebase follows these SOLID principles:

- **S**ingle Responsibility - Each class has one reason to change
- **O**pen/Closed - Open for extension (new strategies), closed for modification
- **L**iskov Substitution - Strategies are interchangeable implementations
- **I**nterface Segregation - Focused, purpose-specific interfaces
- **D**ependency Inversion - Depends on abstractions, not concrete implementations
