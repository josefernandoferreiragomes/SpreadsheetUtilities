# Project Structure & File Organization

## Solution Layout

```
SpreadsheetUtilities.sln
â”‚
â”œâ”€â”€ src/
â”‚   â”œâ”€â”€ SpreadsheetUtility.Domain/          # Pure domain entities & value objects
â”‚   â”œâ”€â”€ SpreadsheetUtility.Application/     # Use cases, DTOs, mappers, ports, services
â”‚   â”œâ”€â”€ SpreadsheetUtility.Library/         # Legacy (being phased out)
â”‚   â”‚   â”œâ”€â”€ Providers/                      # Implement Application.Ports interfaces
â”‚   â”‚   â”œâ”€â”€ Excel/                          # ClosedXML-dependent implementations
â”‚   â”‚   â””â”€â”€ SpreadsheetGenerator/           # Legacy (deferred to Phase 3)
â”‚   â”‚
â”‚   â””â”€â”€ Presentation/
â”‚       â”œâ”€â”€ SpreadsheetUtility.UI.Web/      # Blazor Server app
â”‚       â”œâ”€â”€ SpreadsheetUtility.UI.Console/  # Console app
â”‚       â””â”€â”€ SpreadsheetUtilities.Auth.Api/  # Auth Minimal API
â”‚
â”œâ”€â”€ tests/
â”‚   â””â”€â”€ SpreadsheetUtility.Test/           # All tests (xUnit)
â”‚
â””â”€â”€ shared/
    â”œâ”€â”€ SpreadsheetUtilities.ServiceDefaults/  # Aspire service defaults
    â””â”€â”€ SpreadsheetUtilities.AppHost/          # Aspire orchestrator
```

## Project Dependencies

```
SpreadsheetUtility.Domain          net10.0     [no dependencies]
       â†‘
SpreadsheetUtility.Application     net10.0     [Domain, MediatR, FluentValidation]
       â†‘
SpreadsheetUtility.Library         net10.0     [Domain, Application, ClosedXML, Newtonsoft.Json]
       â†‘
├── SpreadsheetUtility.UI.Web      net10.0     [Application, Infrastructure]
├── SpreadsheetUtility.UI.Console  net10.0     [Application, Infrastructure]
└── SpreadsheetUtilities.Auth.Api  net10.0     [Application, Infrastructure]
```

## Key Folders (Application)

| Path | Description |
|------|-------------|
| `Application/DTOs/` | 7 DTOs: `TaskDto`, `ProjectDto`, `DeveloperDto`, `CalculateGanttChartAllocationInput/Output`, `DeveloperAvailability`, `ListGeneratorInput` |
| `Application/Ports/` | Abstractions: `IDateTimeProvider`, `IHolidayProvider`, `IExcelWorkbook`, `IExcelWorksheet` |
| `Application/Mappers/` | `IGanttChartMapper` / `GanttChartMapper` (DTO â†” Domain) |
| `Application/Services/` | Calculators, strategies, factories, list generators, builders, PasteParserService |
| `Application/UseCases/` | MediatR queries/commands and handlers |
| `Application/Behaviors/` | `LoggingBehavior`, `ValidationBehavior` pipelines |

## Layer Responsibility

| Layer | Responsible For |
|-------|----------------|
| **Domain** | Entities (`GanttTask`, `Developer`, `Project`, `ProjectGroup`, `Holiday`), Value Objects (`DateRange`, `VacationPeriod`), Domain service interfaces, Repository interfaces |
| **Application** | Use case orchestration via MediatR, DTOs, mapping, port abstractions, business logic services (calculators, strategies) |
| **Library** | Infrastructure implementations (providers), legacy code, spreadsheet I/O (ClosedXML), `SpreadsheetGenerator` |
| **Presentation** | Blazor components/pages, API endpoints, console I/O |

## Key Files

| File | Description |
|------|-------------|
| `Application/DependencyInjection.cs` | `AddApplication()` extension for DI registration |
| `Application/Behaviors/LoggingBehavior.cs` | MediatR pipeline: logs requests/responses |
| `Application/Behaviors/ValidationBehavior.cs` | MediatR pipeline: validates inputs via FluentValidation |
| `Application/UseCases/CalculateGanttChartAllocation/` | Query + Handler replacing `GanttChartProcessor` |
| `Application/UseCases/LoadTasks/` | Query + Handler for loading tasks from Excel |
| `Application/UseCases/ParseExcelData/` | Command + Handler for parsing pasted Excel data |
| `Library/Providers/IDateTimeProvider.cs` | Implements `Application.Ports.IDateTimeProvider` |
| `Library/Providers/IHolidayProvider.cs` | Implements `Application.Ports.IHolidayProvider` |
| `Application/Services/IPasteParserService.cs` | Interface for parsing pasted TSV data |
| `Application/Services/PasteParserService.cs` | Implementation: ParseProjects, ParseTasks, ParseTeam |
| `Infrastructure/Services/AuthService.cs` | IAuthService implementation using IMemoryCache |
| `UI.Web/ViewModels/GanttGeneratorViewModel.cs` | Page state holder for GanttGeneratorFromPaste.razor |
