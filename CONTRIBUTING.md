# Contributing Guide

Thank you for your interest in contributing to **SpreadsheetUtilities**! This guide will help you understand how to contribute effectively.

## Table of Contents

1. [Code of Conduct](#code-of-conduct)
2. [Getting Started](#getting-started)
3. [Development Setup](#development-setup)
4. [Making Changes](#making-changes)
5. [Testing](#testing)
6. [Submitting Changes](#submitting-changes)
7. [Style Guide](#style-guide)
8. [Commit Conventions](#commit-conventions)
9. [Pull Request Process](#pull-request-process)
10. [Reporting Bugs](#reporting-bugs)
11. [Suggesting Features](#suggesting-features)

---

## Code of Conduct

This project adheres to the [Contributor Covenant Code of Conduct](https://www.contributor-covenant.org/version/2/1/code_of_conduct/).

### Our Pledge

We are committed to providing a welcoming and inspiring community for all. We expect all contributors to:

- Use welcoming and inclusive language
- Be respectful of differing opinions, viewpoints, and experiences
- Gracefully accept constructive criticism
- Focus on what is best for the community
- Show empathy towards other community members

---

## Getting Started

### Prerequisites

- **.NET 8 SDK** or later
- **.NET 9 SDK** (for Blazor web project)
- **Visual Studio 2022** (Community edition is free) or VS Code with C# extension
- **Git** for version control

### Setup Instructions

1. **Fork the Repository**
   ```bash
   # Go to https://github.com/josefernandoferreiragomes/SpreadsheetUtilities
   # Click "Fork" button
   ```

2. **Clone Your Fork**
   ```bash
   git clone https://github.com/YOUR-USERNAME/SpreadsheetUtilities.git
   cd SpreadsheetUtilities
   ```

3. **Add Upstream Remote**
   ```bash
   git remote add upstream https://github.com/josefernandoferreiragomes/SpreadsheetUtilities.git
   ```

4. **Create Local Development Branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

---

## Development Setup

### Initial Build

```bash
# Restore all dependencies
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test
```

### Running the Web Application Locally

```bash
cd SpreadsheetUtility.UI.Web

# Run development server
dotnet run

# Open browser to https://localhost:5001
```

### Running the Console Application

```bash
cd SpreadsheetUtility/SpreadsheetUtility.UI.Console

# Example usage
dotnet run input.xlsx 2 5 output.xlsx
```

### Running Tests

```bash
# All tests
dotnet test

# Specific test class
dotnet test --filter "DateCalculatorTests"

# With verbose output
dotnet test --logger "console;verbosity=detailed"
```

### Debugging

**Visual Studio:**
- Open solution file (`SpreadsheetUtilities.sln`)
- Set desired breakpoints
- Press F5 to start debugging

**VS Code:**
- Install C# extension
- Use Debug panel (Ctrl+Shift+D)
- Add breakpoints and run

---

## Making Changes

### Branch Strategy

1. **Always start from main**
   ```bash
   git checkout main
   git pull upstream main
   ```

2. **Create feature branch**
   ```bash
   git checkout -b feature/short-description
   # or
   git checkout -b fix/short-description
   ```

3. **Branch naming convention**
   - Features: `feature/add-user-authentication`
   - Fixes: `fix/date-calculation-bug`
   - Chores: `chore/update-dependencies`
   - Docs: `docs/improve-readme`

### Making Your Changes

1. **Modify relevant files** following the style guide (see below)
2. **Add tests** for new functionality
3. **Update documentation** (README, code comments, CHANGELOG)
4. **Keep commits small and logical**

### Files to Update

When making changes, consider updating:

- **Code changes**: Update affected `.cs` files
- **Tests**: Add or update tests in `SpreadsheetUtility.Test/`
- **Documentation**: Update README.md or create new docs
- **Changelog**: Add entry to `CHANGELOG.md` under `[Unreleased]`
- **Instructions**: Update `.copilot-instructions` if conventions change

---

## Testing

### Writing Tests

**Use xUnit and Moq:**

```csharp
[Fact]
public void CalculateWorkingDays_WithWeekendAndHoliday_ExcludesBoth()
{
    // Arrange
    var mockHolidayProvider = new Mock<IHolidayProvider>();
    mockHolidayProvider.Setup(p => p.GetHolidays(2025))
        .Returns(new[] { new Holiday { Date = new DateTime(2025, 1, 1) } });

    var calculator = new DateCalculator(mockHolidayProvider.Object);
    var startDate = new DateTime(2025, 1, 3); // Friday
    var endDate = new DateTime(2025, 1, 6);  // Monday (after weekend)

    // Act
    var result = calculator.CalculateWorkingDays(startDate, endDate);

    // Assert
    Assert.Equal(2, result); // Friday and Monday only
}
```

### Test Naming Convention

```
[MethodName]_[Scenario]_[ExpectedOutcome]
```

Examples:
- `CalculateWorkingDays_WithWeekend_ExcludesWeekendDays`
- `MapToGanttTask_WithValidDto_ReturnsValidGanttTask`
- `ProcessGanttChart_WithNullTasks_ThrowsArgumentNullException`

### Running Tests Before Submitting

```bash
# Run all tests
dotnet test

# Ensure no failures
# Ensure good coverage for new code
```

### Test Coverage Goals

- **New features**: Aim for 80%+ coverage
- **Bug fixes**: Add regression tests
- **Refactoring**: Existing tests should pass

---

## Submitting Changes

### Before You Push

1. **Update CHANGELOG.md**
   ```markdown
   ### Added
   - [Your feature description]

   ### Fixed
   - [Bug fix description]
   ```

2. **Verify your changes compile**
   ```bash
   dotnet build
   ```

3. **Run all tests**
   ```bash
   dotnet test
   ```

4. **Format your code** (Visual Studio handles this automatically)

5. **Review your changes**
   ```bash
   git diff
   ```

### Pushing to Your Fork

```bash
git add .
git commit -m "feat: add new feature"
git push origin feature/your-feature-name
```

### Create a Pull Request

1. Go to your fork on GitHub
2. Click "Compare & pull request"
3. Ensure base repository is: `josefernandoferreiragomes/SpreadsheetUtilities`
4. Ensure base branch is: `main`
5. Fill in the PR template (see below)
6. Click "Create pull request"

### Pull Request Template

```markdown
## Description
Brief description of what this PR does.

## Type of Change
- [ ] Bug fix (non-breaking change)
- [ ] New feature (non-breaking change)
- [ ] Breaking change
- [ ] Documentation update

## Motivation and Context
Why is this change needed? What problem does it solve?

## Related Issue(s)
Closes #123

## How Has This Been Tested?
- [ ] Unit tests added/updated
- [ ] Manual testing completed
- [ ] Tests pass locally

## Checklist
- [ ] My code follows the style guidelines
- [ ] I have performed a self-review of my own code
- [ ] I have commented my code, particularly in hard-to-understand areas
- [ ] I have updated CHANGELOG.md
- [ ] New and existing unit tests pass locally
- [ ] Any dependent changes have been merged and published

## Screenshots (if applicable)
If UI changes, include screenshots.
```

---

## Style Guide

### C# Code Style

**Naming Conventions:**
```csharp
// Classes, Methods, Properties: PascalCase
public class GanttChartProcessor { }
public void ProcessGanttChart() { }
public string ProjectName { get; set; }

// Private fields: _camelCase
private readonly ILogger<MyClass> _logger;
private readonly string _configValue;

// Local variables: camelCase
var taskCount = tasks.Count();
int calculatedDays = calculator.CalculateDays(...);

// Constants: UPPER_SNAKE_CASE
private const int MAX_RETRIES = 3;
private const string DEFAULT_TIMEZONE = "UTC";
```

**Formatting:**
```csharp
// ✅ Good: 4-space indentation, braces on same line
public class MyClass
{
    public void MyMethod()
    {
        if (condition)
        {
            DoSomething();
        }
    }
}

// ✅ Good: Use var when type is obvious
var tasks = new List<GanttTask>();
var processor = new GanttChartProcessor(dependencies);

// ✅ Good: Clear method parameters
public void AssignTasks(
    IEnumerable<GanttTask> tasks,
    IEnumerable<Developer> developers,
    DateTime startDate)
{
    // ...
}
```

**Comments:**
```csharp
// ✅ Good: Explains WHY, not WHAT
// We sort tasks by effort first to ensure dependents
// can be assigned immediately after their dependencies
var sortedTasks = tasks.OrderByDescending(t => t.EffortHours);

// ✅ Good: XML documentation for public APIs
/// <summary>
/// Calculates working days between two dates, excluding weekends and holidays.
/// </summary>
/// <param name="startDate">Start date (inclusive)</param>
/// <param name="endDate">End date (inclusive)</param>
/// <returns>Number of working days</returns>
/// <exception cref="ArgumentException">If endDate is before startDate</exception>
public int CalculateWorkingDays(DateTime startDate, DateTime endDate)
{
    // ...
}

// ❌ Bad: States the obvious
int count = list.Count(); // Get the count

// ❌ Bad: Redundant with code
var x = 5; // Set x to 5
```

**LINQ Formatting:**
```csharp
// ✅ Good: Readable multi-line LINQ
var assignedTasks = tasks
    .Where(t => t.IsAssigned)
    .OrderBy(t => t.DueDate)
    .ThenBy(t => t.Priority)
    .ToList();

// ✅ Good: Method syntax when complex
var groupedByProject = tasks
    .GroupBy(t => t.ProjectId)
    .Select(g => new ProjectSummary
    {
        ProjectId = g.Key,
        TaskCount = g.Count(),
        TotalEffort = g.Sum(t => t.EffortHours)
    })
    .ToList();
```

### Interface Design

```csharp
// ✅ Good: Single responsibility, focused
public interface IDateCalculator
{
    int CalculateWorkingDays(DateTime start, DateTime end);
    DateRange CalculateDateRange(List<Developer> developers);
}

// ❌ Bad: Multiple unrelated responsibilities
public interface IEverything
{
    int CalculateWorkingDays(...);
    void SendEmail(...);
    void SaveToDatabase(...);
    void GenerateReport(...);
}
```

### Error Handling

```csharp
// ✅ Good: Validate inputs, throw descriptive exceptions
public void ProcessTasks(IEnumerable<GanttTask> tasks)
{
    if (tasks == null)
        throw new ArgumentNullException(nameof(tasks));

    if (!tasks.Any())
        throw new ArgumentException("Tasks cannot be empty", nameof(tasks));

    // Process...
}

// ✅ Good: Catch specific exceptions
try
{
    var result = ProcessComplexLogic();
}
catch (ArgumentException ex)
{
    _logger.LogError($"Invalid input: {ex.Message}");
    throw;
}
catch (Exception ex)
{
    _logger.LogError($"Unexpected error: {ex}");
    throw;
}

// ❌ Bad: Catch all exceptions silently
try { ProcessSomething(); } catch { }
```

---

## Commit Conventions

Follow [Conventional Commits](https://www.conventionalcommits.org/):

### Format

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

### Types

- **feat**: New feature
- **fix**: Bug fix
- **docs**: Documentation changes
- **style**: Code style changes (no logic change)
- **refactor**: Code refactoring
- **perf**: Performance improvements
- **test**: Adding/updating tests
- **chore**: Build process, dependencies, tooling

### Examples

```bash
# Feature
git commit -m "feat: add user authentication with OAuth2"

# Bug fix with scope
git commit -m "fix(DateCalculator): correct working days calculation for DST"

# Multiple line commit with body
git commit -m "feat: implement file upload feature

- Add file validation
- Support Excel and CSV formats
- Add progress tracking"

# Breaking change
git commit -m "feat!: change GanttTask DTO structure

BREAKING CHANGE: TaskDto.EffortHours renamed to TaskDto.EstimatedHours"
```

---

## Pull Request Process

### Before Requesting Review

- [ ] Branch is up to date with `main`
- [ ] All tests pass
- [ ] Code compiles without warnings
- [ ] CHANGELOG.md is updated
- [ ] No unnecessary files committed (.vs, bin, obj, etc.)

### Review Process

1. **Maintainer reviews** your code
2. **Changes requested?** Make updates and push to same branch
3. **CI checks pass?** (automated tests)
4. **Approved?** Maintainer merges your PR

### After Merge

Your branch will be deleted automatically. You can clean up locally:

```bash
git checkout main
git pull upstream main
git branch -d feature/your-feature-name
```

---

## Reporting Bugs

### Bug Report Template

Use GitHub Issues with this template:

```markdown
## Description
Clear description of the bug.

## Reproduction Steps
1. Step one
2. Step two
3. Step three

## Expected Behavior
What should happen.

## Actual Behavior
What actually happens.

## Environment
- OS: [Windows/Mac/Linux]
- .NET SDK: [version]
- Visual Studio: [version]

## Error Message/Logs
```
[Paste any error messages or logs]
```

## Screenshots
[If applicable]
```

### Security Vulnerabilities

**Do NOT create a public issue.** Email security concerns privately to maintain responsible disclosure.

---

## Suggesting Features

### Feature Request Template

Use GitHub Discussions or Issues with this template:

```markdown
## Feature Description
What feature would you like to add?

## Motivation
Why would this be useful? What problem does it solve?

## Proposed Solution
How should it work? Do you have a design?

## Alternative Solutions
Have you considered other approaches?

## Additional Context
Any other relevant information.
```

### Good Feature Requests Include

- Clear use case/motivation
- Proposed API or user interface
- Consideration of backward compatibility
- Alignment with project goals

---

## Tips for Successful Contributions

### 1. Start with an Issue
Before investing time in a large feature:
- Check if an issue exists
- Create an issue to discuss your approach
- Wait for feedback before implementing

### 2. Keep PRs Focused
- One feature or fix per PR
- Easier to review
- Easier to revert if needed

### 3. Communication
- Comment on issues you're interested in
- Discuss design decisions
- Ask questions if unclear

### 4. Be Patient
- Maintainers are volunteers
- Reviews take time
- Feedback is meant to help, not criticize

### 5. Learn from the Project
- Study existing design patterns
- Read ARCHITECTURE.md
- Follow established conventions

---

## Common Contribution Scenarios

### Adding a New Design Pattern

1. Identify the problem the pattern solves
2. Study the pattern in ARCHITECTURE.md
3. Implement in library project
4. Add unit tests
5. Document in code and ARCHITECTURE.md
6. Create PR with examples

### Fixing a Bug

1. Create issue describing the bug
2. Add failing unit test (red)
3. Fix the code (green)
4. Refactor if needed (refactor)
5. Ensure all tests pass
6. Update CHANGELOG.md
7. Create PR

### Improving Documentation

1. Fork repository
2. Update .md files with improvements
3. Ensure clarity and accuracy
4. Create PR with clear description

### Adding New Feature to Blazor UI

1. Design the feature
2. Create discussion or issue
3. Implement with tests in mind
4. Update CHANGELOG.md
5. Update README if user-facing
6. Create PR

---

## Questions?

- Check existing issues and discussions
- Read ARCHITECTURE.md and .copilot-instructions
- Ask in GitHub Discussions
- Open an issue for clarification

---

## Additional Resources

- [GitHub Guides - Contributing](https://guides.github.com/activities/contributing-to-open-source/)
- [How to Write the Perfect Pull Request](https://github.blog/2015-01-21-how-to-write-the-perfect-pull-request/)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)

---

**Thank you for contributing to SpreadsheetUtilities!** 🎉

Your efforts help make this project better for everyone.
