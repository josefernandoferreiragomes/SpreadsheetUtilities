# Project Structure & File Organization

## Solution Layout

```
SpreadsheetUtilities.sln
│
├── src/
│   ├── SpreadsheetUtility.Domain/          # Pure domain entities & value objects
│   ├── SpreadsheetUtility.Application/     # Use cases, DTOs, mappers, ports, services
│   ├── SpreadsheetUtility.Library/         # Legacy (being phased out)
│   │   ├── Providers/                      # Implement Application.Ports interfaces
│   │   ├── Excel/                          # ClosedXML-dependent implementations
│   │   └── SpreadsheetGenerator/           # Legacy (deferred to Phase 3)
│   │
│   └── Presentation/
│       ├── SpreadsheetUtility.UI.Web/      # Blazor Server app
│       ├── SpreadsheetUtility.UI.Console/  # Console app
│       └── SpreadsheetUtilities.Auth.Api/  # Auth Minimal API
│
├── tests/
│   └── SpreadsheetUtility.Test/           # All tests (xUnit)
│
└── shared/
    ├── SpreadsheetUtilities.ServiceDefaults/  # Aspire service defaults
    └── SpreadsheetUtilities.AppHost/          # Aspire orchestrator
```

## Project Dependencies

```
SpreadsheetUtility.Domain          net10.0     [no dependencies]
       ↑
SpreadsheetUtility.Application     net10.0     [Domain, MediatR, FluentValidation]
       ↑
SpreadsheetUtility.Library         net10.0     [Domain, Application, ClosedXML, Newtonsoft.Json]
       ↑
├── SpreadsheetUtility.UI.Web      net10.0     [Library, Application, Newtonsoft.Json]
├── SpreadsheetUtility.UI.Console  net10.0     [Library]
└── SpreadsheetUtilities.Auth.Api  net10.0     [Library, Aspire]
```

## Key Folders (Application)

| Path | Description |
|------|-------------|
| `Application/DTOs/` | 7 DTOs: `TaskDto`, `ProjectDto`, `DeveloperDto`, `CalculateGanttChartAllocationInput/Output`, `DeveloperAvailability`, `ListGeneratorInput` |
| `Application/Ports/` | Abstractions: `IDateTimeProvider`, `IHolidayProvider`, `IExcelWorkbook`, `IExcelWorksheet` |
| `Application/Mappers/` | `IGanttChartMapper` / `GanttChartMapper` (DTO ↔ Domain) |
| `Application/Services/` | Calculators, strategies, factories, list generators, builders |
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
