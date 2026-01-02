# SpreadsheetUtilities

> Comprehensive Excel-based project management and data transformation suite with Gantt chart visualization

![.NET](https://img.shields.io/badge/.NET-8%2B%209-blue)
![License](https://img.shields.io/badge/license-MIT-green)
![Status](https://img.shields.io/badge/status-Active-brightgreen)

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Projects in This Solution](#projects-in-this-solution)
- [Installation & Setup](#installation--setup)
- [Usage](#usage)
  - [Double Entry Spreadsheet Generator](#double-entry-spreadsheet-generator)
  - [Gantt Chart Generator](#gantt-chart-generator)
- [Architecture & Design Patterns](#architecture--design-patterns)
- [Development](#development)
- [Contributing](#contributing)
- [License](#license)

---

## 🎯 Overview

**SpreadsheetUtilities** is a comprehensive solution for Excel-based data transformation and project visualization. It provides:

1. **Double Entry Spreadsheet Generator** - Transform single-entry spreadsheets with multi-line cells into clean double-entry format
2. **Gantt Chart Generator** - Visualize project schedules with automatic task assignment to team members considering vacation periods and dependencies
3. **Reusable Library** - Battle-tested, design-pattern-driven library for spreadsheet processing and project management

The solution combines console applications, a Blazor web interface, and a robust core library following SOLID principles and multiple industry-standard design patterns.

---

## ✨ Features

### Double Entry Spreadsheet Generator
- ✅ Converts single-entry spreadsheets with multi-line cells into double-entry format
- ✅ Customizable key and value columns
- ✅ Optional header row specification
- ✅ Multi-worksheet support
- ✅ Batch processing capability
- ✅ Available as console app or portable executable

### Gantt Chart Generator
- ✅ **Interactive Web UI** (Blazor Server)
- ✅ **Paste-based Data Entry** - Copy/paste data directly from Excel
- ✅ **Automatic Task Assignment** - Intelligent task scheduling to team members
- ✅ **Vacation & Holiday Handling** - Respects developer vacation periods and company holidays
- ✅ **Task Dependencies** - Support for inter-task dependencies within projects
- ✅ **Multiple Views**:
  - Task assignments Gantt chart
  - Project-level Gantt chart
  - Developer workload Gantt chart
- ✅ **Advanced Options**:
  - Pre-sort tasks by effort (useful for dependency resolution)
  - Fix teams to project groups for better resource allocation
  - Configurable project start date
  - Week or Day view modes
- ✅ **Data Analytics**:
  - Project effort totals
  - Developer allocation tracking
  - Slack time calculation
  - Vacation day accounting
  - Non-working day tracking
- ✅ **Visual Features**:
  - Color-coded projects and developers
  - Real-time data tables with sorting
  - Holiday tracking across projects

---

## 📦 Projects in This Solution

### 1. **SpreadsheetUtility.Library** (.NET 8)
Core business logic and reusable components for both console and web applications.

**Key Components:**
- `GanttChartProcessor` - Main orchestrator for task allocation and scheduling
- `DateCalculator` - Advanced date calculations with holiday support
- `GanttChartMapper` - DTO to domain model transformations
- `TaskAssignment & Sorting Strategies` - Pluggable algorithm implementations
- `Holiday & DateTime Providers` - External dependency abstraction

### 2. **SpreadsheetUtility.UI.Web** (.NET 9 - Blazor Server)
Interactive web application for Gantt chart generation with real-time data visualization.

**Features:**
- Paste-based Excel data import
- QuickGrid-based data tables with sorting/filtering
- Frappe Gantt chart visualization
- Multiple chart modes (Week/Day view)
- Advanced configuration options
- Real-time developer workload analysis

**Routes:**
- `/` - Home page
- `/ganttGeneratorFromPaste` - Interactive Gantt generator
- `/jsonGeneratorFromPaste` - JSON data generator (experimental)

### 3. **SpreadsheetUtility.UI.Console** (.NET 8)
Console-based double entry spreadsheet generator.

**Usage:**
```bash
dotnet run <input.xlsx> <key column> <values column> [output.xlsx] [headers row] [worksheet index]
```

### 4. **SimplifiedUtilityConsole** (.NET 8)
Lightweight console application for basic spreadsheet transformations.

### 5. **SpreadsheetUtility.Test** (.NET 8)
Comprehensive unit test suite for library components using xUnit and Moq.

---

## 🚀 Installation & Setup

### Prerequisites

- **.NET 8 SDK** (for console apps and library)
- **.NET 9 SDK** (for Blazor web application)
- **Visual Studio 2022** or VS Code with C# extension (recommended)
- **Git** for cloning the repository

### Clone Repository

```bash
git clone https://github.com/josefernandoferreiragomes/SpreadsheetUtilities.git
cd SpreadsheetUtilities
```

### Install Dependencies

```bash
# Restore all NuGet packages
dotnet restore

# Build entire solution
dotnet build
```

### Required NuGet Packages (already included in projects)

```bash
# Excel processing
dotnet add package ClosedXML

# Gantt chart visualization
dotnet add package Newtonsoft.Json

# Data grid components (Blazor)
dotnet add package Microsoft.AspNetCore.Components.QuickGrid

# Testing
dotnet add package xunit
dotnet add package Moq

# Dependency injection
dotnet add package Microsoft.Extensions.Hosting
dotnet add package Microsoft.Extensions.Hosting.Abstractions
```

---

## 💻 Usage

### Double Entry Spreadsheet Generator

#### Console Application

**From Visual Studio Developer PowerShell:**

```bash
# Navigate to console project
cd .\SpreadsheetUtility\SpreadsheetUtility.UI.Console

# Run with parameters
dotnet run input.xlsx 2 5 output.xlsx
```

**Parameters:**
- `<input.xlsx>` - Path to input spreadsheet file
- `<key column>` - Column index (1-based) containing keys
- `<values column>` - Column index (1-based) containing multi-line values
- `[output.xlsx]` (optional) - Output file path (default: auto-generated in same directory)
- `[headers row]` (optional) - Header row index (default: 1)
- `[worksheet index]` (optional) - Worksheet index (default: 1)

**Example:**

```bash
dotnet run .\ExampleFiles\input.xlsx 2 5 output.xlsx 1 1
```

**Input Format Example:**

| Id | CarModel | Features |
|---|---|---|
| 1 | CarModelA | AirConditioning<br>PowerSteering |
| 2 | CarModelB | PowerSteering<br>BucketSeats |

**Output Format:**

| Id | CarModel | AirConditioning | PowerSteering | BucketSeats |
|---|---|---|---|---|
| 1 | CarModelA | X | X | |
| 2 | CarModelB | | X | X |

#### As Portable Executable

```bash
# Publish console app
dotnet publish -c Release --self-contained

# Navigate to output directory
cd .\bin\Release\net8.0\win-x64\publish

# Execute
.\SpreadsheetUtilityConsole.exe input.xlsx 2 5 output.xlsx
```

---

### Gantt Chart Generator

#### Web Application (Recommended)

**Start the Application:**

```bash
# Navigate to web project
cd .\SpreadsheetUtility.UI.Web

# Run development server
dotnet run

# Open browser
# Navigate to: https://localhost:5001/ganttGeneratorFromPaste
```

**Step-by-Step Usage:**

1. **Prepare Your Data**
   - Export or copy your project data from Excel
   - Export task/effort data with columns: ProjectID, TaskName, EffortHours, etc.
   - Export team data with columns: TeamId, Name, DailyWorkHours, VacationPeriods

2. **Enter Project Data**
   - Paste project spreadsheet data in the "Projects" textarea
   - Specify project groups for batch assignment
   - Set team IDs to link developers to project groups (optional)

3. **Enter Task Data**
   - Paste task spreadsheet data in the "Tasks" textarea
   - Include: InternalID, TaskName, ProjectName, EffortHours, Dependencies
   - Dependencies reference InternalID of dependent tasks (experimental)
   - Progress is specified as percentage (0-100)

4. **Enter Team Data**
   - Paste developer/team spreadsheet data in the "Team" textarea
   - Format: TeamId, Team, Name, DailyWorkHours, VacationPeriods
   - Vacation periods: comma-separated date pairs (yyyy-MM-dd;yyyy-MM-dd), pipe-separated intervals
   - Example: `2025-12-15;2025-12-20|2026-01-05;2026-01-10`

5. **Configure Options**
   - **Project Start Date** - When project allocation begins
   - **Chart Mode** - Week or Day view
   - **Pre-Sort Tasks** - Enable if tasks have dependencies (experimental)
   - **Team to Project Group** - Fix developers to specific project groups (experimental)

6. **Generate Charts**
   - Click "Load Tasks Gantt Chart" to generate visualization
   - Three Gantt charts will be rendered:
     - **Tasks Chart** - Individual task assignments
     - **Projects Chart** - Project-level overview
     - **Developer Tasks Chart** - Developer workload visualization

7. **Review Data**
   - **Project Data Table** - Summary of all projects with dates and effort
   - **Task Data Table** - Detailed task assignments with working day calculations
   - **Developer Data Table** - Resource allocation and slack time analysis
   - **Holidays Table** - All holidays occurring during project timeline

#### Data Input Format

**Projects Data (Tab-Separated):**
```
ProjectID   ProjectName   ProjectGroup   TeamId
P001        Website Redesign   1   TEAM-A
P002        Mobile App   2   TEAM-B
```

**Tasks Data (Tab-Separated):**
```
InternalID   ProjectName   TaskName   EffortHours   Dependencies   Progress
1   Website Redesign   Design   40   0   50
2   Website Redesign   Development   80   1   30
3   Mobile App   Design   30   0   100
```

**Team Data (Tab-Separated):**
```
TeamId   Team   Name   DailyWorkHours   VacationPeriods
TEAM-A   Team A   John Doe   8   2025-12-24;2025-12-26
TEAM-A   Team A   Jane Smith   8   
TEAM-B   Team B   Bob Johnson   8   2025-01-01;2025-01-03
```

---

## 🏗️ Architecture & Design Patterns

The **SpreadsheetUtility.Library** project implements 11 industry-standard design patterns for maintainability and testability.

### 1. Observer Pattern

**Location:** `DateCalculator.cs`, `GanttChartProcessor.cs`

Implements notification mechanism for holiday detection events.

**Key Components:**
- `IObserver<Holiday>` interface for holiday notifications
- `AddObserver()` / `RemoveObserver()` in `DateCalculator`
- `GanttChartProcessor` implements `IObserver<Holiday>`

**Advantages:**
- Decouples holiday detection from handling
- Multiple independent observers
- Reactive programming support
- Easy observer addition without code changes

---

### 2. Strategy Pattern

**Location:** Task assignment and sorting strategies

Enables runtime switching between different algorithms.

**Key Components:**
- `ITaskAssignmentStrategy` with `DefaultTaskAssignmentStrategy`
- `ITaskSortingStrategy` with `DefaultTaskSortingStrategy` and `TaskSortingStrategyEffortBased`
- Base classes provide common functionality

**Advantages:**
- Runtime algorithm switching
- New strategies without modifying existing code
- Independently testable
- Open/Closed Principle compliance

---

### 3. Factory Pattern

**Location:** Strategy factories

Centralizes strategy instantiation with dependency injection.

**Key Components:**
- `ITaskAssignmentStrategyFactory` / `TaskAssignmentStrategyFactory`
- `ITaskSortingStrategyFactory` / `TaskSortingStrategyFactory`

**Advantages:**
- Decouples instantiation from usage
- Centralized creation logic
- DI integration
- Testability improvement

---

### 4. Template Method Pattern

**Location:** `TaskAssignmentStrategyBase.cs`, `ListGenerator<TInput, TOutput>`

Base classes define skeleton algorithms, subclasses implement specific steps.

**Advantages:**
- Code reuse across implementations
- Consistent operation structure
- Reduced duplication
- Explicit algorithm structure

---

### 5. Builder Pattern

**Location:** `CalculateGanttChartAllocationOutputBuilder.cs`

Simplifies construction of complex objects.

**Key Components:**
- Fluent API with method chaining
- `WithProjects()`, `WithGanttTasks()`, etc.
- `Build()` method finalizer

**Advantages:**
- Improved readability
- Elegant complex initialization
- Immutable result objects
- Clear optional parameters

---

### 6. Facade Pattern

**Location:** `CalculatorFacade.cs`, `ICalculatorFacade.cs`

Provides unified interface to multiple calculator services.

**Advantages:**
- Reduced complexity
- Single service entry point
- Easy extension
- Client decoupling

---

### 7. Mapper/Adapter Pattern

**Location:** `GanttChartMapper.cs`, `IGanttChartMapper.cs`

Transforms between DTOs and domain models.

**Key Transformations:**
- TaskDto → GanttTask
- ProjectDto → Project
- DeveloperDto → Developer
- Developer → DeveloperAvailability

**Advantages:**
- DTO/Domain model decoupling
- Clean transformations
- Centralized mapping logic
- Easier testing

---

### 8. Generic List Generator Pattern

**Location:** `IListGenerator<TInput, TOutput>`

Type-safe, reusable grouping and aggregation.

**Key Components:**
- `GanttTaskProjectListGenerator` - GanttTask → Project
- `GanttTaskListGenerator` - GanttTask → GanttTask
- `DeveloperTaskListGenerator` - Developer → List<GanttTask>

**Advantages:**
- Type safety with generics
- Code reuse
- Independent testability
- Easy extension

---

### 9. Command Pattern

**Location:** `ILogCommand.cs`, logging implementations

Encapsulates logging requests for deferred execution.

**Key Components:**
- `ILogCommand` interface
- `ProcessorMessageLogCommand`
- `CalculateGanttChartAllocationInputLogCommand`
- `CalculateGanttChartAllocationOutputLogCommand`
- `LoggingInvoker` for queue management

**Advantages:**
- Request encapsulation
- Batch logging operations
- Deferred execution
- Easy new command addition

---

### 10. Dependency Injection Pattern

**Location:** Throughout library and Program.cs

Constructor injection for loose coupling.

**Key Components:**
- Service registration in `Program.cs`
- Constructor injection in all services
- `IServiceProvider` for runtime resolution

**Advantages:**
- Improved testability
- Reduced coupling
- Centralized configuration
- Easy service substitution

---

### 11. Provider Pattern

**Location:** `IHolidayProvider.cs`, `IDateTimeProvider.cs`

Abstracts external dependencies.

**Key Components:**
- `IHolidayProvider` - Holiday data provisioning
- `IDateTimeProvider` - Date/time abstraction
- Mock-friendly implementations

**Advantages:**
- External dependency abstraction
- Easy testing with mocks
- Business logic isolation
- Multiple implementation support

---

## Design Principles Applied

The codebase follows **SOLID principles**:

| Principle | Application |
|-----------|-------------|
| **S**ingle Responsibility | Each class has one reason to change |
| **O**pen/Closed | Open for extension (new strategies), closed for modification |
| **L**iskov Substitution | Strategies are interchangeable implementations |
| **I**nterface Segregation | Focused, purpose-specific interfaces |
| **D**ependency Inversion | Depends on abstractions, not concrete implementations |

---

## 🛠️ Development

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test .\SpreadsheetUtility\SpreadsheetUtility.Test\SpreadsheetUtility.Test.csproj

# Run with verbose output
dotnet test --logger "console;verbosity=detailed"

# Run specific test class
dotnet test --filter "DateCalculatorTests"
```

### Building and Publishing

```bash
# Build solution
dotnet build

# Build release configuration
dotnet build -c Release

# Publish console app
dotnet publish -c Release --self-contained -r win-x64
```

### Project Structure

```
SpreadsheetUtilities/
├── SpreadsheetUtility/
│   ├── SpreadsheetUtility.Library/          # Core business logic
│   │   ├── Calculators/                     # Date, hours calculations
│   │   ├── Mappers/                         # DTO transformations
│   │   ├── Providers/                       # Holiday, DateTime abstractions
│   │   ├── Processors/                      # GanttChartProcessor
│   │   ├── TaskAssigners/                   # Task assignment strategies
│   │   ├── TaskSorters/                     # Task sorting strategies
│   │   ├── ListGenerators/                  # Generic list generation
│   │   └── Models/                          # DTOs and domain models
│   ├── SpreadsheetUtility.UI.Console/       # Console app
│   └── SpreadsheetUtility.Test/             # Unit tests
├── SpreadsheetUtility.UI.Web/               # Blazor web app
│   ├── Components/
│   │   ├── Pages/                           # Razor pages
│   │   └── Layout/                          # Layout components
│   ├── Helpers/                             # Utility helpers
│   └── wwwroot/                             # Static assets
└── SimplifiedUtilityConsole/                # Legacy console app
```

---

## 🤝 Contributing

Contributions are welcome! Please follow these guidelines:

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/amazing-feature
   ```
3. **Commit your changes**
   ```bash
   git commit -m 'Add amazing feature'
   ```
4. **Push to the branch**
   ```bash
   git push origin feature/amazing-feature
   ```
5. **Open a Pull Request**

### Code Style

- Follow C# coding conventions
- Use meaningful variable/method names
- Add unit tests for new features
- Document complex logic with comments
- Follow existing pattern implementations

### Reporting Issues

Please use the GitHub Issues tab to report bugs or request features with:
- Clear description
- Steps to reproduce
- Expected vs. actual behavior
- Screenshots if applicable

---

## 📝 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE.txt) file for details.

All dependencies maintain their respective licenses:
- **ClosedXML**: MIT License
- **Frappe Gantt**: MIT License
- **xUnit**: Apache 2.0 License
- **Moq**: BSD License

---

## 📞 Support

For questions, issues, or suggestions:
- Open an issue on [GitHub Issues](https://github.com/josefernandoferreiragomes/SpreadsheetUtilities/issues)
- Check existing documentation in this README
- Review unit tests for usage examples

---

## 🙏 Acknowledgments

- **Frappe Gantt** - Open-source Gantt chart visualization library
- **ClosedXML** - Modern Excel manipulation library
- **xUnit** - .NET testing framework
- **.NET Community** - For excellent frameworks and tools

---

## 📚 Additional Resources

- [Frappe Gantt Documentation](https://github.com/frappe/gantt)
- [ClosedXML Documentation](https://github.com/ClosedXML/ClosedXML)
- [Blazor Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [Design Patterns in C#](https://refactoring.guru/design-patterns)

---

**Built with ❤️ for the community**
