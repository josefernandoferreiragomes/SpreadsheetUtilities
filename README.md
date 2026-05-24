# SpreadsheetUtilities

> Comprehensive Excel-based project management and data transformation suite with Gantt chart visualization

![.NET](https://img.shields.io/badge/.NET-10-blue)
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
3. **Layered Architecture** - Clean Architecture with Domain, Application, and Library layers following SOLID principles and multiple industry-standard design patterns

The solution combines console applications, a Blazor web interface, and a layered architecture following Clean Architecture and Domain-Driven Design principles.

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

### 1. **SpreadsheetUtility.Domain** (.NET 10)
Pure domain model with zero dependencies. Contains entities, value objects, domain service interfaces, and repository interfaces.

**Key Components:**
- `Developer`, `GanttTask`, `Project`, `ProjectGroup`, `Holiday` entities
- `DateRange`, `VacationPeriod` value objects
- `IDateCalculationService`, `IHolidayLookupService` domain service interfaces
- `IHolidayRepository`, `IDeveloperRepository` repository interfaces

### 2. **SpreadsheetUtility.Application** (.NET 10)
Use-case orchestration layer using MediatR. Contains DTOs, mappers, port abstractions, calculators, strategies, and pipeline behaviors.

**Key Components:**
- MediatR use cases: `CalculateGanttChartAllocation`, `LoadTasks`, `ParseExcelData`
- DTOs: `TaskDto`, `ProjectDto`, `DeveloperDto`, input/output classes
- `GanttChartMapper` - DTO ↔ Domain mapping
- Port interfaces: `IDateTimeProvider`, `IHolidayProvider`, `IExcelWorkbook`
- Calculator services: `DateCalculator`, `DeveloperHoursCalculator`, `CalculatorFacade`
- Strategy pattern: Task assignment & sorting strategies with factories
- Pipeline behaviors: `LoggingBehavior`, `ValidationBehavior`

### 3. **SpreadsheetUtility.Library** (.NET 10)
Infrastructure implementations and legacy code. Implements port interfaces from the Application layer.

**Key Components:**
- `DateTimeProvider` / `HolidayProvider` - implements `Application.Ports` interfaces
- `ExcelWorkbook` - ClosedXML-based spreadsheet I/O
- `SpreadsheetGenerator` - Legacy double-entry generator (to be moved in Phase 3)

### 4. **SpreadsheetUtility.UI.Web** (.NET 10 - Blazor Server)
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
- `/examplefiles` - Example file browser and download

### 5. **SpreadsheetUtility.UI.Console** (.NET 10)
Console-based double entry spreadsheet generator.

**Usage:**
```bash
dotnet run <input.xlsx> <key column> <values column> [output.xlsx] [headers row] [worksheet index]
```

### 6. **SpreadsheetUtility.Test** (.NET 10)
Comprehensive unit test suite (26 tests) using xUnit and Moq.

---

## 🚀 Installation & Setup

### Prerequisites

- **.NET 10 SDK**
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

### Key NuGet Packages (already included in projects)

| Package | Used By | Purpose |
|---------|---------|---------|
| `ClosedXML` | Library | Excel spreadsheet I/O |
| `Newtonsoft.Json` | Library, Web | JSON serialization (Gantt chart data) |
| `MediatR` | Application | Use-case orchestration |
| `FluentValidation` | Application | Input validation |
| `Microsoft.AspNetCore.Components.QuickGrid` | UI.Web | Sortable data tables |
| `xunit` / `Moq` | Test | Unit testing |

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

The solution follows **Clean Architecture** with three main layers:

```
Presentation (UI.Web, UI.Console, Auth.Api)
       ↓
Application (Use Cases, DTOs, Mappers, Ports)
       ↓
Domain (Entities, Value Objects, Services)
       ↑
Library (Infrastructure implementations, legacy code)
```

### 1. Observer Pattern

**Location:** `DateCalculator.cs`, `CalculateGanttChartAllocationQueryHandler.cs`

Holiday detection notifications using `IObserver<Holiday>`.

**Key Components:**
- `AddObserver()` / `RemoveObserver()` in `DateCalculator`
- `CalculateGanttChartAllocationQueryHandler` implements `IObserver<Holiday>`

---

### 2. Strategy Pattern

**Location:** `Application/Services/` — task assignment and sorting strategies

Runtime-switchable algorithms for task allocation and sorting.

**Key Components:**
- `ITaskAssignmentStrategy` / `DefaultTaskAssignmentStrategy`
- `ITaskSortingStrategy` / `DefaultTaskSortingStrategy` / `TaskSortingStrategyEffortBased`

---

### 3. Factory Pattern

**Location:** `Application/Services/` — `TaskAssignmentStrategyFactory`, `TaskSortingStrategyFactory`

Centralizes strategy instantiation via DI.

---

### 4. Template Method Pattern

**Location:** `TaskAssignmentStrategyBase.cs`, `ListGenerator<TInput, TOutput>`

Base classes define skeleton algorithms; subclasses fill in specific steps.

---

### 5. Builder Pattern

**Location:** `CalculateGanttChartAllocationOutputBuilder.cs`

Fluent API for constructing complex output objects with method chaining.

---

### 6. Facade Pattern

**Location:** `CalculatorFacade.cs`

Unified interface over `DateCalculator`, `DeveloperHoursCalculator`, and related services.

---

### 7. Mapper/Adapter Pattern

**Location:** `GanttChartMapper.cs`

Transforms DTOs (`TaskDto`, `ProjectDto`, `DeveloperDto`) to Domain entities and back.

---

### 8. MediatR Pipeline Behavior Pattern

**Location:** `Application/Behaviors/`

Replaces the legacy Command pattern for cross-cutting concerns. Uses MediatR pipeline behaviors for:
- `LoggingBehavior<TRequest, TResponse>` — logs every request/response
- `ValidationBehavior<TRequest, TResponse>` — validates inputs via FluentValidation

**Advantages:**
- Declarative cross-cutting concerns
- No modification to handler code
- Ordered pipeline execution
- Reusable across all use cases

---

### 9. Generic List Generator Pattern

**Location:** `IListGenerator<TInput, TOutput>` implementations

Type-safe, reusable grouping and aggregation using generics.

**Implementations:**
- `GanttTaskProjectListGenerator` — GanttTask → Project
- `GanttTaskListGenerator` — GanttTask → GanttTask
- `DeveloperTaskListGenerator` — Developer → List<GanttTask>

---

### 10. Dependency Injection Pattern

**Location:** Throughout all layers

Constructor injection with centralized registration via `AddApplication()` extension method and `Program.cs`.

---

### 11. Provider Pattern

**Location:** `Application/Ports/` and `Library/Providers/`

Abstractions in the Application layer, implementations in Library:
- `IDateTimeProvider` → `DateTimeProvider`
- `IHolidayProvider` → `HolidayProvider`

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
│   ├── SpreadsheetUtility.Domain/           # Pure domain (zero dependencies)
│   │   ├── Models/                          # Entities
│   │   ├── ValueObjects/                    # Value objects
│   │   └── Services/                        # Domain service interfaces
│   │
│   ├── SpreadsheetUtility.Application/      # Use-case orchestration
│   │   ├── DTOs/                            # Data transfer objects
│   │   ├── Ports/                           # Abstractions (IDateTimeProvider, etc.)
│   │   ├── Mappers/                         # DTO ↔ Domain mapping
│   │   ├── Services/                        # Calculators, strategies, builders
│   │   ├── Behaviors/                       # MediatR pipeline behaviors
│   │   └── UseCases/                        # MediatR queries/commands + handlers
│   │
│   ├── SpreadsheetUtility.Library/          # Infrastructure + legacy
│   │   ├── Providers/                       # DateTimeProvider, HolidayProvider
│   │   ├── Excel/                           # ClosedXML implementations
│   │   └── SpreadsheetGenerator/            # Legacy double-entry generator
│   │
│   ├── SpreadsheetUtility.UI.Console/       # Console app
│   └── SpreadsheetUtility.Test/             # Unit tests (xUnit)
│
├── SpreadsheetUtility.UI.Web/               # Blazor web app
│   ├── Components/
│   │   ├── Pages/                           # Razor pages
│   │   └── Layout/                          # Layout components
│   ├── Helpers/                             # Utility helpers
│   └── wwwroot/                             # Static assets
│
├── SpreadsheetUtilities.Auth.Api/           # Minimal API auth
├── SpreadsheetUtilities.ServiceDefaults/    # Aspire service defaults
└── SpreadsheetUtilities.AppHost/            # Aspire orchestrator
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
