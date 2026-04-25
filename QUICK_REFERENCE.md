# Quick Reference Guide

Fast lookup for common tasks and conventions in SpreadsheetUtilities development.

## Table of Contents

- [Project Setup](#project-setup)
- [Common Commands](#common-commands)
- [Code Patterns](#code-patterns)
- [File Organization](#file-organization)
- [Naming Conventions](#naming-conventions)
- [Testing](#testing)
- [Git Workflow](#git-workflow)
- [Troubleshooting](#troubleshooting)

---

## Project Setup

### First Time Setup

```bash
# Clone repository
git clone https://github.com/josefernandoferreiragomes/SpreadsheetUtilities.git
cd SpreadsheetUtilities

# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test
```

### Opening in Visual Studio

1. Open `SpreadsheetUtilities.sln`
2. Wait for solution to load (Intellisense indexing)
3. Press F5 to start debugging web app (or select project and run)

### Running Web App Locally

```bash
cd SpreadsheetUtility.UI.Web
dotnet run
# Navigate to https://localhost:5001
```

---

## Common Commands

### Building

```bash
# Restore + Build all
dotnet build

# Release configuration
dotnet build -c Release

# Build specific project
dotnet build SpreadsheetUtility/SpreadsheetUtility.Library/

# Clean build
dotnet clean && dotnet build
```

### Testing

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "DateCalculatorTests"

# Run with output
dotnet test --logger "console;verbosity=detailed"

# Run single test method
dotnet test --filter "FullyQualifiedName~DateCalculatorTests.CalculateWorkingDays_WithWeekend"
```

### NuGet Package Management

```bash
# Add package
dotnet add package PackageName --version 1.0.0

# Update packages
dotnet outdated  # List outdated packages
dotnet upgrade   # Update all packages (in .NET 7+)

# Remove package
dotnet remove package PackageName
```

### Database (EF Core)

```bash
# Add new migration
dotnet ef migrations add MigrationName -p SpreadsheetUtility\SpreadsheetUtility.Library.DataAccess

# List migrations
dotnet ef migrations list -p SpreadsheetUtility\SpreadsheetUtility.Library.DataAccess

# Update database
dotnet ef database update -p SpreadsheetUtility\SpreadsheetUtility.Library.DataAccess

# Remove last migration
dotnet ef migrations remove -p SpreadsheetUtility\SpreadsheetUtility.Library.DataAccess

# Generate SQL script
dotnet ef migrations script -p SpreadsheetUtility\SpreadsheetUtility.Library.DataAccess
```

### Publishing

```bash
cd SpreadsheetUtility.UI.Web

# Publish to folder
dotnet publish -c Release -o ./publish

# Publish with specific runtime
dotnet publish -c Release -r win-x64 --self-contained

# Publish to Azure App Service
az webapp deployment source config-zip --resource-group myGroup --name myApp --src publish.zip
```

---

## Code Patterns

### Creating a New Service

```csharp
// 1. Create interface
public interface IMyService
{
    Task<Result> DoSomethingAsync(Input input);
}

// 2. Create implementation
public class MyService : IMyService
{
    private readonly ILogger<MyService> _logger;
    private readonly IDependency _dependency;

    // Constructor injection
    public MyService(IDependency dependency, ILogger<MyService> logger)
    {
        _dependency = dependency;
        _logger = logger;
    }

    public async Task<Result> DoSomethingAsync(Input input)
    {
        _logger.LogInformation("Starting operation");

        // Validate
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        // Execute
        var result = await _dependency.ProcessAsync(input);

        _logger.LogInformation("Operation completed");
        return result;
    }
}

// 3. Register in Program.cs
builder.Services.AddScoped<IMyService, MyService>();

// 4. Use in components/services
public class MyComponent
{
    private readonly IMyService _myService;

    public MyComponent(IMyService myService)
    {
        _myService = myService;
    }

    public async Task DoWork()
    {
        var result = await _myService.DoSomethingAsync(new Input());
    }
}
```

### Creating a New Strategy

```csharp
// 1. Create strategy interface (if not exists)
public interface IMyStrategy
{
    void Execute(Input input);
}

// 2. Create base class (optional, for shared logic)
public abstract class MyStrategyBase : IMyStrategy
{
    protected virtual void ValidateInput(Input input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));
    }

    public void Execute(Input input)
    {
        ValidateInput(input);
        PerformWork(input);
    }

    protected abstract void PerformWork(Input input);
}

// 3. Create specific implementations
public class DefaultMyStrategy : MyStrategyBase
{
    protected override void PerformWork(Input input)
    {
        // Default algorithm
    }
}

public class AdvancedMyStrategy : MyStrategyBase
{
    protected override void PerformWork(Input input)
    {
        // Advanced algorithm
    }
}

// 4. Create factory
public interface IMyStrategyFactory
{
    IMyStrategy CreateStrategy(string strategyName);
}

public class MyStrategyFactory : IMyStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;

    public MyStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IMyStrategy CreateStrategy(string strategyName)
    {
        return strategyName switch
        {
            "default" => _serviceProvider.GetRequiredService<DefaultMyStrategy>(),
            "advanced" => _serviceProvider.GetRequiredService<AdvancedMyStrategy>(),
            _ => throw new ArgumentException($"Unknown strategy: {strategyName}")
        };
    }
}

// 5. Register in Program.cs
builder.Services.AddScoped<DefaultMyStrategy>();
builder.Services.AddScoped<AdvancedMyStrategy>();
builder.Services.AddScoped<IMyStrategyFactory, MyStrategyFactory>();

// 6. Use
public class MyProcessor
{
    private readonly IMyStrategyFactory _strategyFactory;

    public MyProcessor(IMyStrategyFactory strategyFactory)
    {
        _strategyFactory = strategyFactory;
    }

    public void Process(Input input, string strategyName)
    {
        var strategy = _strategyFactory.CreateStrategy(strategyName);
        strategy.Execute(input);
    }
}
```

### Writing Tests

```csharp
[Fact]
public void MethodName_Scenario_ExpectedOutcome()
{
    // Arrange
    var mockDependency = new Mock<IDependency>();
    mockDependency.Setup(d => d.GetData())
        .Returns(new Data { Value = 42 });

    var sut = new ServiceUnderTest(mockDependency.Object);
    var input = new TestInput { Id = 1 };

    // Act
    var result = sut.ProcessInput(input);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(42, result.Value);
    mockDependency.Verify(d => d.GetData(), Times.Once);
}
```

---

## File Organization

### Library Project Structure

```
SpreadsheetUtility.Library/
├── Calculators/
│   ├── ICalculator.cs (interface)
│   ├── Calculator.cs (implementation)
│   └── CalculatorBase.cs (base class if needed)
├── Processors/
│   ├── IProcessor.cs
│   └── Processor.cs
├── Strategies/
│   ├── IStrategy.cs
│   ├── StrategyBase.cs
│   ├── DefaultStrategy.cs
│   └── StrategyFactory.cs
├── Models/
│   ├── Domain/
│   │   ├── Entity.cs
│   │   └── ValueObject.cs
│   └── DTOs/
│       ├── InputDto.cs
│       └── OutputDto.cs
├── Mappers/
│   ├── IMapper.cs
│   └── Mapper.cs
├── Providers/
│   ├── IProvider.cs
│   └── Provider.cs
└── Services/
    ├── IService.cs
    └── Service.cs
```

### Web Project Structure

```
SpreadsheetUtility.UI.Web/
├── Components/
│   ├── Pages/
│   │   ├── Index.razor
│   │   ├── GanttGenerator.razor
│   │   └── Login.razor
│   ├── Layout/
│   │   └── MainLayout.razor
│   └── Shared/
│       └── NavMenu.razor
├── Services/
│   ├── IApplicationService.cs
│   └── ApplicationService.cs
├── Helpers/
│   └── ValidationHelper.cs
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── lib/
├── Program.cs
├── appsettings.json
└── appsettings.Development.json
```

---

## Naming Conventions

### C# Files & Classes

```
File: MyService.cs
Namespace: SpreadsheetUtility.Library.Services
Class: public class MyService { }

File: IMyService.cs
Interface: public interface IMyService { }
```

### Variables & Properties

```csharp
// Constants
private const int MAX_RETRIES = 3;
private const string DEFAULT_NAME = "Default";

// Private fields (backing fields)
private readonly IService _service;
private int _count;

// Properties
public string Name { get; set; }
public int Count { get; private set; }

// Local variables
var taskList = GetTasks();
int calculatedValue = CalculateValue();
foreach (var item in items) { }
```

### Methods

```csharp
// Async methods end with Async
public async Task<Result> ProcessAsync() { }

// Boolean methods start with Is, Has, Can, or Should
public bool IsValid() { }
public bool HasPermission() { }
public bool CanProcess() { }

// Query methods start with Get
public User GetUser(int id) { }

// Command methods use action verbs
public void Create(User user) { }
public void Update(User user) { }
public void Delete(int id) { }
```

### Database Models

```csharp
// Table name: Plural
public class User { }           // → Users table
public class Project { }        // → Projects table

// Navigation properties
public ICollection<Project> Projects { get; set; }
public User User { get; set; }

// Foreign keys
public int UserId { get; set; }
public User User { get; set; } // Shadow foreign key
```

---

## Testing

### Test File Organization

```
SpreadsheetUtility.Test/
├── Calculators/
│   └── DateCalculatorTests.cs
├── Processors/
│   └── GanttChartProcessorTests.cs
├── Mappers/
│   └── GanttChartMapperTests.cs
├── Helpers/
│   └── TestDataBuilder.cs
│   └── MockFactory.cs
└── TestBase.cs
```

### Test Naming Pattern

```
[MethodName]_[Scenario]_[ExpectedOutcome]

Examples:
- CalculateWorkingDays_WithWeekend_ExcludesWeekendDays
- MapToGanttTask_WithValidDto_ReturnsValidTask
- ProcessGanttChart_WithNullTasks_ThrowsArgumentNullException
```

### Common Assertions

```csharp
// Equality
Assert.Equal(expected, actual);
Assert.NotEqual(expected, actual);

// Null checking
Assert.Null(value);
Assert.NotNull(value);

// Boolean
Assert.True(condition);
Assert.False(condition);

// Collections
Assert.Empty(collection);
Assert.NotEmpty(collection);
Assert.Contains(item, collection);
Assert.Single(collection);

// Exceptions
Assert.Throws<ArgumentNullException>(() => method(null));
```

---

## Git Workflow

### Creating Feature Branch

```bash
# Update main
git checkout main
git pull upstream main

# Create feature branch
git checkout -b feature/my-feature

# Make changes...
git add .
git commit -m "feat: add my feature"

# Push to fork
git push origin feature/my-feature

# Create Pull Request on GitHub
```

### Keeping Branch Updated

```bash
# Fetch latest from upstream
git fetch upstream

# Rebase on latest main
git rebase upstream/main

# If conflicts, resolve them
# git add .
# git rebase --continue

# Force push to your fork (only for local branches!)
git push origin feature/my-feature -f
```

### Squashing Commits

```bash
# Interactive rebase (last 3 commits)
git rebase -i HEAD~3

# Mark commits as 'squash' (or 's') to combine with previous
# Save and close editor
# Edit commit message if needed

# Force push (only safe on feature branches!)
git push origin feature/my-feature -f
```

---

## Troubleshooting

### Build Issues

**Error**: "The type or namespace name 'X' does not exist"

**Solutions**:
```bash
# Restore packages
dotnet restore

# Clean and rebuild
dotnet clean
dotnet build

# Check project references
# Right-click project → Properties → References
```

### Test Failures

**Error**: "Mock setup returns null"

**Solution**:
```csharp
// ✅ Correct: Set up return value
mock.Setup(x => x.GetData()).Returns(new Data());

// ❌ Wrong: No setup
mock.Setup(x => x.GetData()); // Returns default
```

### Database Issues

**Error**: "Migrations pending"

**Solution**:
```bash
dotnet ef database update -p SpreadsheetUtility\SpreadsheetUtility.Library.DataAccess
```

**Error**: "Cannot open local database"

**Solution**:
```bash
# Ensure LocalDB is running (Windows)
sqllocaldb start mssqllocaldb

# Or use in-memory database for testing
```

### Dependency Injection Issues

**Error**: "Cannot resolve service"

**Solution**:
```csharp
// ✅ Correct: Register in Program.cs
builder.Services.AddScoped<IMyService, MyService>();

// ❌ Wrong: Not registered
// Will throw exception when injected

// ✅ Also correct: Check registration
// builder.Services.AddScoped<MyService>(); // If not using interface
```

### NuGet Package Issues

**Error**: "Package not found"

**Solution**:
```bash
# Check NuGet.org
# Update NuGet source
dotnet nuget add source https://api.nuget.org/v3/index.json

# Clear cache
dotnet nuget locals all --clear
```

### Blazor Issues

**Error**: "Cascading parameter is null"

**Solution**:
```razor
<!-- ✅ Correct: Pass cascading parameter -->
<CascadingValue Value="this">
    <ChildComponent />
</CascadingValue>

<!-- In ChildComponent.razor -->
[CascadingParameter] public ParentComponent? Parent { get; set; }

<!-- ✅ Check for null -->
@if (Parent != null)
{
    <p>Parent exists</p>
}
```

---

## Useful Links

- **Documentation**: `.copilot-instructions`
- **Architecture**: `ARCHITECTURE.md`
- **Contributing**: `CONTRIBUTING.md`
- **Implementation Plan**: `IMPLEMENTATION_ROADMAP.md`
- **Changelog**: `CHANGELOG.md`

---

## Quick Checklist Before PR

- [ ] Code compiles without warnings
- [ ] All tests pass (`dotnet test`)
- [ ] New code has 80%+ test coverage
- [ ] Follows naming conventions
- [ ] Comments explain "why", not "what"
- [ ] CHANGELOG.md updated
- [ ] No unnecessary files committed
- [ ] Branch is up to date with main
- [ ] Commit messages follow conventions
- [ ] PR description is clear

---

**Last Updated**: [Current Session]
**For Help**: See CONTRIBUTING.md or open an issue
