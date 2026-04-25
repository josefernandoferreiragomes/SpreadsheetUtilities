# Architecture Guide - SpreadsheetUtilities

This document provides an in-depth look at the architectural decisions, design patterns, and principles that guide the SpreadsheetUtilities project.

## Table of Contents

1. [Project Structure](#project-structure)
2. [Design Principles](#design-principles)
3. [Design Patterns](#design-patterns)
4. [Layered Architecture](#layered-architecture)
5. [Data Flow](#data-flow)
6. [Future Architecture](#future-architecture)

---

## Project Structure

```
SpreadsheetUtilities/
├── SpreadsheetUtility/
│   ├── SpreadsheetUtility.Library/          # Core calculator library (.NET 8)
│   │   ├── Calculators/
│   │   │   ├── DateCalculator.cs
│   │   │   ├── IDeveloperHoursCalculator.cs
│   │   │   ├── CalculatorFacade.cs
│   │   │   └── ICalculatorFacade.cs
│   │   ├── Mappers/
│   │   │   ├── GanttChartMapper.cs
│   │   │   └── IGanttChartMapper.cs
│   │   ├── Providers/
│   │   │   ├── IHolidayProvider.cs
│   │   │   ├── IDateTimeProvider.cs
│   │   │   ├── HolidayProvider.cs
│   │   │   └── DateTimeProvider.cs
│   │   ├── Processors/
│   │   │   ├── GanttChartProcessor.cs
│   │   │   ├── IGanttChartProcessor.cs
│   │   │   ├── GanttChartDataManager.cs
│   │   │   └── IGanttChartDataManager.cs
│   │   ├── TaskAssigners/
│   │   │   ├── ITaskAssignmentStrategy.cs
│   │   │   ├── TaskAssignmentStrategyBase.cs
│   │   │   ├── DefaultTaskAssignmentStrategy.cs
│   │   │   ├── ITaskAssignmentStrategyFactory.cs
│   │   │   └── TaskAssignmentStrategyFactory.cs
│   │   ├── TaskSorters/
│   │   │   ├── ITaskSortingStrategy.cs
│   │   │   ├── TaskSortingStrategyBase.cs
│   │   │   ├── DefaultTaskSortingStrategy.cs
│   │   │   ├── TaskSortingStrategyEffortBased.cs
│   │   │   ├── ITaskSortingStrategyFactory.cs
│   │   │   └── TaskSortingStrategyFactory.cs
│   │   ├── ListGenerators/
│   │   │   ├── IListGenerator<TInput, TOutput>.cs
│   │   │   ├── GanttTaskProjectListGenerator.cs
│   │   │   ├── GanttTaskListGenerator.cs
│   │   │   └── DeveloperTaskListGenerator.cs
│   │   ├── Services/
│   │   │   ├── GroupProjectsByProjectGroupQuery.cs
│   │   │   └── LoggingInvoker.cs
│   │   ├── Models/
│   │   │   ├── DTOs/
│   │   │   └── Domain/
│   │   ├── Domain/
│   │   └── Groupers/
│   │
│   ├── SpreadsheetUtility.Library.DataAccess/ [PLANNED]
│   │   ├── DbContext/
│   │   ├── Repositories/
│   │   ├── Migrations/
│   │   └── Configurations/
│   │
│   ├── SpreadsheetUtility.Library.Identity/ [PLANNED]
│   │   ├── Services/
│   │   ├── Models/
│   │   └── Policies/
│   │
│   ├── SpreadsheetUtility.UI.Console/       # Console app (.NET 8)
│   │   ├── Program.cs
│   │   └── Helpers/
│   │
│   └── SpreadsheetUtility.Test/             # Unit tests (.NET 8)
│       ├── Calculators/
│       ├── Processors/
│       ├── Mappers/
│       └── Helpers/
│
├── SpreadsheetUtility.UI.Web/               # Blazor Server (.NET 9)
│   ├── Components/
│   │   ├── Pages/
│   │   ├── Layout/
│   │   └── Shared/
│   ├── Helpers/
│   ├── Services/
│   ├── wwwroot/
│   ├── Program.cs
│   └── appsettings.json
│
├── SimplifiedUtilityConsole/                # Legacy console app (.NET 8)
│
├── .copilot-instructions                    # Development guidelines
├── CHANGELOG.md                             # Version history
├── ARCHITECTURE.md                          # This file
├── CONTRIBUTING.md                          # Contribution guide
└── README.md                                # Project overview
```

### Why This Structure?

1. **Library Isolation**: Business logic completely separated from UI frameworks
2. **Multiple UIs**: Console and Web apps can share the same library
3. **Testability**: Library can be tested independently
4. **Reusability**: Library can be published as NuGet package
5. **Scalability**: New UI projects can use the same library
6. **Future Layers**: DataAccess and Identity layers keep concerns separate

---

## Design Principles

### SOLID Principles

#### Single Responsibility Principle (SRP)
Each class has ONE reason to change.

```csharp
// ✅ Good: Each class does one thing
public class DateCalculator : IDateCalculator
{
    // Only responsible for date calculations
    public int CalculateWorkingDays(...) { ... }
}

public class GanttChartMapper : IGanttChartMapper
{
    // Only responsible for DTO transformations
    public GanttTask MapToGanttTask(TaskDto dto) { ... }
}

// ❌ Bad: Multiple reasons to change
public class AllInOneCalculator
{
    public int CalculateWorkingDays(...) { ... }
    public GanttTask MapToGanttTask(...) { ... }
    public void SaveToDatabase(...) { ... }
}
```

#### Open/Closed Principle (OCP)
Open for extension, closed for modification.

```csharp
// ✅ Good: New algorithms don't modify existing code
public interface ITaskAssignmentStrategy
{
    IEnumerable<GanttTask> Assign(IEnumerable<GanttTask> tasks);
}

public class DefaultTaskAssignmentStrategy : ITaskAssignmentStrategy { ... }
public class AdvancedTaskAssignmentStrategy : ITaskAssignmentStrategy { ... }

// ❌ Bad: Adding new algorithm requires modifying existing code
public class TaskAssigner
{
    public void Assign(string strategyType)
    {
        if (strategyType == "default") { ... }
        else if (strategyType == "advanced") { ... }
        else if (strategyType == "new") { ... } // Always adding conditions
    }
}
```

#### Liskov Substitution Principle (LSP)
Subtypes must be substitutable for their base types.

```csharp
// ✅ Good: Any strategy can replace another without breaking code
ITaskAssignmentStrategy strategy = strategyFactory.CreateStrategy("default");
var result = strategy.Assign(tasks); // Works regardless of implementation
```

#### Interface Segregation Principle (ISP)
Clients should not depend on interfaces they don't use.

```csharp
// ✅ Good: Focused interfaces
public interface IDateCalculator
{
    int CalculateWorkingDays(DateTime start, DateTime end);
}

public interface IDeveloperHoursCalculator
{
    decimal CalculateAllocatedHours(Developer developer);
}

// ❌ Bad: Fat interface
public interface ICalculator
{
    int CalculateWorkingDays(...);
    decimal CalculateAllocatedHours(...);
    void SendEmail(...);
    void SaveToDatabase(...);
}
```

#### Dependency Inversion Principle (DIP)
High-level modules depend on abstractions, not concrete implementations.

```csharp
// ✅ Good: Depends on abstraction
public class GanttChartProcessor
{
    private readonly IDateCalculator _dateCalculator;

    public GanttChartProcessor(IDateCalculator dateCalculator)
    {
        _dateCalculator = dateCalculator; // Injected abstraction
    }
}

// ❌ Bad: Depends on concrete implementation
public class GanttChartProcessor
{
    private readonly DateCalculator _dateCalculator = new DateCalculator(); // Concrete class
}
```

### Additional Principles

**Don't Repeat Yourself (DRY)**
- Extracted common logic into base classes
- Template Method pattern in TaskAssignmentStrategyBase
- Reusable calculators and utilities

**KISS (Keep It Simple, Stupid)**
- Avoid over-engineering for "future needs"
- Add complexity only when solving real problems
- Clear naming and straightforward logic

**YAGNI (You Aren't Gonna Need It)**
- No speculative features
- Only implement what's actually needed
- Refactor when needs arise

---

## Design Patterns

The project intentionally implements these 11+ design patterns as a learning resource:

### 1. Observer Pattern

**Purpose**: Notify multiple objects about state changes without coupling them.

**Location**: `DateCalculator`, `GanttChartProcessor`

**Implementation**:
```csharp
public interface IObserver<T>
{
    void OnNext(T value);
    void OnError(Exception error);
    void OnCompleted();
}

public class DateCalculator : IObservable<Holiday>
{
    private readonly List<IObserver<Holiday>> _observers = new();

    public IDisposable Subscribe(IObserver<Holiday> observer)
    {
        _observers.Add(observer);
        return new Unsubscriber(_observers, observer);
    }

    private void NotifyHolidayDetected(Holiday holiday)
    {
        foreach (var observer in _observers)
            observer.OnNext(holiday);
    }
}

public class GanttChartProcessor : IObserver<Holiday>
{
    public void OnNext(Holiday holiday) { /* Handle detected holiday */ }
    public void OnError(Exception error) { /* Handle error */ }
    public void OnCompleted() { /* Handle completion */ }
}
```

**Benefits**:
- Decouples holiday detection from handling
- Multiple independent observers can react
- Reactive programming model
- Easy to add new observers without modifying existing code

---

### 2. Strategy Pattern

**Purpose**: Define a family of algorithms, encapsulate each one, and make them interchangeable.

**Location**: `ITaskAssignmentStrategy`, `ITaskSortingStrategy`

**Implementation**:
```csharp
public interface ITaskAssignmentStrategy
{
    IEnumerable<GanttTask> Assign(
        IEnumerable<GanttTask> tasks,
        IEnumerable<Developer> developers);
}

public class DefaultTaskAssignmentStrategy : ITaskAssignmentStrategy
{
    public IEnumerable<GanttTask> Assign(...) { /* Default algorithm */ }
}

public class AdvancedTaskAssignmentStrategy : ITaskAssignmentStrategy
{
    public IEnumerable<GanttTask> Assign(...) { /* Advanced algorithm */ }
}

// Usage: Switch algorithms at runtime
ITaskAssignmentStrategy strategy = strategyFactory.CreateStrategy(userChoice);
var assignedTasks = strategy.Assign(tasks, developers);
```

**Benefits**:
- Runtime algorithm switching
- New algorithms without modifying existing code
- Easy testing with different strategies
- Clear separation of concerns

---

### 3. Factory Pattern

**Purpose**: Create objects without specifying concrete classes.

**Location**: `TaskAssignmentStrategyFactory`, `TaskSortingStrategyFactory`

**Implementation**:
```csharp
public interface ITaskAssignmentStrategyFactory
{
    ITaskAssignmentStrategy CreateStrategy(string strategyName);
}

public class TaskAssignmentStrategyFactory : ITaskAssignmentStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TaskAssignmentStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ITaskAssignmentStrategy CreateStrategy(string strategyName)
    {
        return strategyName switch
        {
            "default" => _serviceProvider.GetRequiredService<DefaultTaskAssignmentStrategy>(),
            "advanced" => _serviceProvider.GetRequiredService<AdvancedTaskAssignmentStrategy>(),
            _ => throw new ArgumentException($"Unknown strategy: {strategyName}")
        };
    }
}
```

**Benefits**:
- Centralized object creation
- Easy to add new strategy types
- Dependency Injection integration
- Single place to change instantiation logic

---

### 4. Template Method Pattern

**Purpose**: Define skeleton of algorithm in base class, let subclasses override specific steps.

**Location**: `TaskAssignmentStrategyBase`, `TaskSortingStrategyBase`

**Implementation**:
```csharp
public abstract class TaskAssignmentStrategyBase : ITaskAssignmentStrategy
{
    public IEnumerable<GanttTask> Assign(
        IEnumerable<GanttTask> tasks,
        IEnumerable<Developer> developers)
    {
        // Template method defines algorithm skeleton
        var sortedTasks = SortTasks(tasks);
        var validatedTasks = ValidateInputs(sortedTasks);
        var assignedTasks = PerformAssignment(validatedTasks, developers);
        return PostProcessAssignments(assignedTasks);
    }

    protected virtual void SortTasks(IEnumerable<GanttTask> tasks) 
        => throw new NotImplementedException();

    protected virtual void ValidateInputs(IEnumerable<GanttTask> tasks) 
        => throw new NotImplementedException();

    protected abstract void PerformAssignment(
        IEnumerable<GanttTask> tasks,
        IEnumerable<Developer> developers);

    protected virtual void PostProcessAssignments(IEnumerable<GanttTask> tasks)
        => tasks; // Default: no post-processing
}

public class DefaultTaskAssignmentStrategy : TaskAssignmentStrategyBase
{
    protected override void PerformAssignment(...) 
    { 
        // Specific implementation details
    }
}
```

**Benefits**:
- Consistent algorithm structure
- Code reuse in base class
- Forced implementation of key steps
- Clear extension points for subclasses

---

### 5. Builder Pattern

**Purpose**: Construct complex objects step-by-step.

**Location**: `CalculateGanttChartAllocationOutputBuilder`

**Implementation**:
```csharp
public class CalculateGanttChartAllocationOutputBuilder
{
    private List<Project> _projects;
    private List<GanttTask> _ganttTasks;
    private List<Developer> _developers;
    private List<Holiday> _holidays;

    public CalculateGanttChartAllocationOutputBuilder WithProjects(List<Project> projects)
    {
        _projects = projects;
        return this;
    }

    public CalculateGanttChartAllocationOutputBuilder WithGanttTasks(List<GanttTask> tasks)
    {
        _ganttTasks = tasks;
        return this;
    }

    public CalculateGanttChartAllocationOutputBuilder WithDevelopers(List<Developer> developers)
    {
        _developers = developers;
        return this;
    }

    public CalculateGanttChartAllocationOutput Build()
    {
        return new CalculateGanttChartAllocationOutput(
            _projects,
            _ganttTasks,
            _developers,
            _holidays);
    }
}

// Usage: Fluent API
var output = new CalculateGanttChartAllocationOutputBuilder()
    .WithProjects(projects)
    .WithGanttTasks(tasks)
    .WithDevelopers(developers)
    .Build();
```

**Benefits**:
- Cleaner object initialization
- Optional parameters without overloads
- Improved readability
- Immutable result objects

---

### 6. Facade Pattern

**Purpose**: Provide unified interface to subsystem.

**Location**: `CalculatorFacade`, `IGanttChartDataManager`

**Implementation**:
```csharp
public interface ICalculatorFacade
{
    DateRange CalculateDateRange(...);
    int CalculateWorkingDays(...);
    decimal CalculateDevHours(...);
}

public class CalculatorFacade : ICalculatorFacade
{
    private readonly IDateCalculator _dateCalculator;
    private readonly IDeveloperHoursCalculator _hoursCalculator;

    public CalculatorFacade(
        IDateCalculator dateCalculator,
        IDeveloperHoursCalculator hoursCalculator)
    {
        _dateCalculator = dateCalculator;
        _hoursCalculator = hoursCalculator;
    }

    public DateRange CalculateDateRange(...) 
        => _dateCalculator.CalculateDateRange(...);

    public int CalculateWorkingDays(...)
        => _dateCalculator.CalculateWorkingDays(...);

    public decimal CalculateDevHours(...)
        => _hoursCalculator.CalculateAllocatedHours(...);
}
```

**Benefits**:
- Simplified client interface
- Hides subsystem complexity
- Single entry point for related operations
- Easy to extend without affecting clients

---

### 7. Mapper/Adapter Pattern

**Purpose**: Transform objects from one format to another.

**Location**: `GanttChartMapper`

**Implementation**:
```csharp
public interface IGanttChartMapper
{
    GanttTask MapToGanttTask(TaskDto taskDto);
    Project MapToProject(ProjectDto projectDto);
    Developer MapToDeveloper(DeveloperDto developerDto);
}

public class GanttChartMapper : IGanttChartMapper
{
    public GanttTask MapToGanttTask(TaskDto taskDto)
    {
        return new GanttTask
        {
            Id = taskDto.Id,
            Name = taskDto.TaskName,
            ProjectId = taskDto.ProjectId,
            // ... more mappings
        };
    }

    public Developer MapToDeveloper(DeveloperDto developerDto)
    {
        return new Developer
        {
            Id = developerDto.TeamId,
            Name = developerDto.Name,
            // ... more mappings
        };
    }
}
```

**Benefits**:
- DTO/Domain model separation
- Centralized transformation logic
- Easy to modify mappings
- Testable transformations

---

### 8. Generic List Generator Pattern

**Purpose**: Type-safe, reusable grouping and aggregation.

**Location**: `IListGenerator<TInput, TOutput>`

**Implementation**:
```csharp
public interface IListGenerator<TInput, TOutput>
{
    IEnumerable<TOutput> Generate(IEnumerable<TInput> items);
}

public class GanttTaskProjectListGenerator : IListGenerator<GanttTask, Project>
{
    public IEnumerable<Project> Generate(IEnumerable<GanttTask> tasks)
    {
        return tasks
            .GroupBy(t => t.ProjectId)
            .Select(g => new Project { Id = g.Key, Tasks = g.ToList() });
    }
}

public class DeveloperTaskListGenerator : IListGenerator<Developer, List<GanttTask>>
{
    public IEnumerable<List<GanttTask>> Generate(IEnumerable<Developer> developers)
    {
        return developers
            .Select(d => d.AssignedTasks.ToList());
    }
}
```

**Benefits**:
- Type safety with generics
- Code reuse across different types
- Independent testability
- Easy to add new generators

---

### 9. Command Pattern

**Purpose**: Encapsulate requests as objects for deferred execution.

**Location**: `ILogCommand`, `LoggingInvoker`

**Implementation**:
```csharp
public interface ILogCommand
{
    void Execute();
}

public class ProcessorMessageLogCommand : ILogCommand
{
    private readonly ILogger<GanttChartProcessor> _logger;
    private readonly string _message;

    public ProcessorMessageLogCommand(ILogger<GanttChartProcessor> logger, string message)
    {
        _logger = logger;
        _message = message;
    }

    public void Execute() => _logger.LogInformation(_message);
}

public class LoggingInvoker
{
    private readonly Queue<ILogCommand> _commandQueue = new();

    public void EnqueueCommand(ILogCommand command)
    {
        _commandQueue.Enqueue(command);
    }

    public void ExecuteAll()
    {
        while (_commandQueue.TryDequeue(out var command))
        {
            command.Execute();
        }
    }
}
```

**Benefits**:
- Request encapsulation
- Batch logging operations
- Deferred execution
- Easy to add new command types

---

### 10. Dependency Injection Pattern

**Purpose**: Provide dependencies to objects rather than having them create their own.

**Location**: Throughout `Program.cs` and all constructors

**Implementation**:
```csharp
// Program.cs setup
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IGanttChartProcessor, GanttChartProcessor>();
builder.Services.AddScoped<IDateCalculator, DateCalculator>();
builder.Services.AddScoped<IHolidayProvider, HolidayProvider>();

// Component/Service usage
public class GanttChartComponent
{
    private readonly IGanttChartProcessor _processor;

    // Dependencies injected via constructor
    public GanttChartComponent(IGanttChartProcessor processor)
    {
        _processor = processor;
    }
}
```

**Benefits**:
- Loose coupling
- Improved testability
- Centralized configuration
- Easy to swap implementations

---

### 11. Provider Pattern

**Purpose**: Abstract external dependencies.

**Location**: `IHolidayProvider`, `IDateTimeProvider`

**Implementation**:
```csharp
public interface IHolidayProvider
{
    IEnumerable<Holiday> GetHolidays(int year);
}

public interface IDateTimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}

public class HolidayProvider : IHolidayProvider
{
    public IEnumerable<Holiday> GetHolidays(int year)
    {
        return new List<Holiday>
        {
            new Holiday { Date = new DateTime(year, 1, 1), Name = "New Year" },
            new Holiday { Date = new DateTime(year, 12, 25), Name = "Christmas" },
            // ... more holidays
        };
    }
}

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}

// Testing: Mock implementation
public class MockHolidayProvider : IHolidayProvider
{
    public IEnumerable<Holiday> GetHolidays(int year) 
        => new List<Holiday>(); // No holidays for testing
}
```

**Benefits**:
- External dependency abstraction
- Easy testing with mocks
- Business logic isolation
- Multiple implementation support

---

## Layered Architecture

The application follows a **3-4 layer architecture**:

```
┌─────────────────────────────────────┐
│  Presentation Layer                 │
│  (Blazor Components, Console UI)    │
├─────────────────────────────────────┤
│  Application/Service Layer [Future] │
│  (Business logic orchestration)     │
├─────────────────────────────────────┤
│  Domain/Business Logic Layer        │
│  (Calculators, Processors,          │
│   Strategies, Mappers)              │
├─────────────────────────────────────┤
│  Data Access Layer [Future]         │
│  (Repositories, DbContext, EF Core) │
└─────────────────────────────────────┘
```

### Layer Responsibilities

#### Presentation Layer (UI)
- Blazor components for web application
- Console interface for command-line app
- User input handling and validation
- Display of results
- **File**: `SpreadsheetUtility.UI.Web/`, `SpreadsheetUtility.UI.Console/`

#### Domain/Business Logic Layer
- Pure calculation logic (no frameworks, no UI)
- Business rules and algorithms
- Design pattern implementations
- Model transformations
- **File**: `SpreadsheetUtility.Library/`
- **Principle**: Should be reusable in any context (console, web, API, mobile)

#### Data Access Layer [Future]
- Database interactions (Entity Framework Core)
- Repository implementations
- Migration management
- **File**: `SpreadsheetUtility.Library.DataAccess/`

#### Application/Service Layer [Future]
- Orchestration of domain services
- Use case implementations
- Transaction management
- **File**: New services in `SpreadsheetUtility.UI.Web/Services/`

### Layer Dependencies

```
Presentation Layer
        ↓
Application/Service Layer [Future]
        ↓
Domain/Business Logic Layer
        ↓
Data Access Layer [Future]
```

**Key Rule**: Never depend upward. A lower layer should never know about upper layers.

---

## Data Flow

### Current Data Flow (Single Request)

```
User Interface
    ↓
[Paste/Upload Data]
    ↓
Blazor Component
    ↓
Input Validation & Parsing
    ↓
GanttChartMapper (DTO → Domain)
    ↓
GanttChartProcessor
    ├─→ DateCalculator
    ├─→ TaskAssignmentStrategy
    ├─→ ListGenerators
    └─→ GanttChartDataManager
    ↓
CalculateGanttChartAllocationOutput
    ↓
Display Results
    ├─→ Gantt Charts (Frappe Gantt)
    ├─→ Data Tables (QuickGrid)
    └─→ Analytics
```

### Future Data Flow (With Persistence)

```
User Interface
    ↓
[Authenticate User]
    ↓
[Load Session Data from Database]
    ↓
Paste/Upload/Local File
    ↓
[Validate & Parse]
    ↓
[Save to Session Cache]
    ↓
Blazor Component
    ↓
[...existing flow...]
    ↓
[Save Results to Database]
    ↓
Display Results
```

### File Processing Flow [Future]

```
User Uploads File
    ↓
File Validation (type, size, format)
    ↓
Parse Excel/CSV
    ↓
Extract Tables
    ↓
Map to DTOs
    ↓
[Proceed with existing flow]
```

---

## Future Architecture

### Phase 1: Add Authentication & Persistence

```
New Projects:
├── SpreadsheetUtility.Library.Identity
│   ├── User management
│   ├── Authentication logic
│   └── Authorization policies
└── SpreadsheetUtility.Library.DataAccess
    ├── DbContext
    ├── Repository<T> implementations
    ├── Unit of Work pattern
    └── EF Core migrations
```

### Phase 2: Add Application Services Layer

```
Enhanced Web Project:
├── Services/
│   ├── IGanttChartApplicationService
│   ├── ISessionService
│   ├── IProjectService
│   └── IFileProcessingService
├── DTOs/
│   ├── CreateProjectRequest
│   ├── UpdateProjectRequest
│   └── ProjectResponse
└── Middleware/
    ├── ExceptionHandling
    ├── RequestValidation
    └── Logging
```

### Phase 3: Add REST API Layer

```
New Project: SpreadsheetUtility.UI.Api
├── Controllers/
│   ├── ProjectsController
│   ├── TasksController
│   └── ReportsController
├── OpenAPI (Swagger)
└── Authentication (JWT tokens)
```

---

## Testing Strategy

### Unit Testing
- **Target**: Library layer (100% public API coverage goal)
- **Framework**: xUnit
- **Mocking**: Moq
- **Location**: `SpreadsheetUtility.Test/`

### Integration Testing [Future]
- **Target**: Database interactions, service layer
- **Database**: SQLite in-memory for tests
- **Patterns**: Repository, Unit of Work

### Component Testing [Future]
- **Target**: Blazor components
- **Framework**: Bunit
- **Mocks**: Mocked services

### End-to-End Testing [Future]
- **Target**: Full workflows
- **Tool**: Playwright or Selenium
- **Scenarios**: Authentication, file upload, data persistence

---

## Dependency Graph

```
External Dependencies:
├── ClosedXML (Excel)
├── Newtonsoft.Json (JSON)
├── Microsoft.Extensions.DependencyInjection
├── Microsoft.Extensions.Logging
├── xUnit (Testing)
├── Moq (Testing)
└── Frappe Gantt (JavaScript, visualization)

Internal Dependencies:
SpreadsheetUtility.Library [No external deps]
    ↑
    ├─ SpreadsheetUtility.UI.Console
    ├─ SpreadsheetUtility.UI.Web
    └─ SpreadsheetUtility.Test

[Future]
SpreadsheetUtility.Library.DataAccess
    ├─ EF Core, SQL Server/SQLite
    └─ Depends on: SpreadsheetUtility.Library

SpreadsheetUtility.Library.Identity
    ├─ ASP.NET Identity
    └─ Depends on: SpreadsheetUtility.Library
```

---

## Extensibility Points

### Adding New Task Assignment Strategy

```csharp
// 1. Create new class implementing ITaskAssignmentStrategy
public class CustomTaskAssignmentStrategy : TaskAssignmentStrategyBase
{
    protected override void PerformAssignment(...) { /* ... */ }
}

// 2. Register in Program.cs
builder.Services.AddScoped<CustomTaskAssignmentStrategy>();

// 3. Add to factory (TaskAssignmentStrategyFactory.cs)
public ITaskAssignmentStrategy CreateStrategy(string strategyName)
{
    return strategyName switch
    {
        "default" => _serviceProvider.GetRequiredService<DefaultTaskAssignmentStrategy>(),
        "custom" => _serviceProvider.GetRequiredService<CustomTaskAssignmentStrategy>(), // New
        _ => throw new ArgumentException(...)
    };
}
```

### Adding New Calculator

```csharp
// 1. Create interface and implementation
public interface ICustomCalculator
{
    decimal Calculate(...);
}

public class CustomCalculator : ICustomCalculator
{
    public decimal Calculate(...) { /* ... */ }
}

// 2. Register in Program.cs
builder.Services.AddScoped<ICustomCalculator, CustomCalculator>();

// 3. Inject into components/services that need it
public class MyComponent
{
    private readonly ICustomCalculator _customCalculator;

    public MyComponent(ICustomCalculator customCalculator)
    {
        _customCalculator = customCalculator;
    }
}
```

### Adding New Blazor Page

```csharp
// 1. Create .razor file in Components/Pages/
// MyNewPage.razor

@page "/my-new-page"
@inject IGanttChartProcessor GanttProcessor
@inject IMapper Mapper

<h1>My New Feature</h1>

// 2. Add logic and UI components
```

---

## Performance Considerations

### Current Implementation
- Single-threaded Blazor Server
- In-memory session state
- No caching layer

### Future Optimizations
- Distributed caching (Redis) for large datasets
- Background job processing (Hangfire) for heavy computations
- Pagination for large result sets
- Query optimization with EF Core

---

## Security Architecture

### Current State
- No authentication required
- Input validation needed in UI layer

### Future State [Planned]
- User authentication (Entra ID or email-based)
- Authorization policies per operation
- Encrypted data at rest and in transit
- Audit logging for sensitive operations
- SQL injection prevention (EF Core parameterized queries)

---

## Deployment Architecture

### Local Development
```
Developer Machine
    ↓
Git Clone
    ↓
dotnet restore
    ↓
dotnet run (Blazor Server)
    ↓
Local SQLite Database [Future]
```

### Azure Production
```
Azure Web App
    ↓
    ├─ Application code (.NET 9)
    ├─ Azure SQL Database [Future]
    ├─ Azure Key Vault (secrets)
    ├─ Application Insights (logging)
    └─ Azure Storage (file uploads) [Future]
```

---

## References

- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [Design Patterns: Elements of Reusable Object-Oriented Software](https://en.wikipedia.org/wiki/Design_Patterns)
- [Refactoring Guru - Design Patterns](https://refactoring.guru/design-patterns)
- [Microsoft Dependency Injection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)

---

**Last Updated**: [Current Session]
**Reviewed By**: Development Team
**Next Review**: After Phase 1 completion
