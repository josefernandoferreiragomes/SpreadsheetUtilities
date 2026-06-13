# Project Structure & File Organization

## Solution Layout

```
SpreadsheetUtilities.sln
│
├── SpreadsheetUtility.Domain/          # Pure domain entities & value objects
├── SpreadsheetUtility.Application/     # Use cases, DTOs, mappers, ports, services
├── SpreadsheetUtility.Infrastructure/  # All I/O, ClosedXML, repos, providers, API clients
├── SpreadsheetUtility.Bootstrapper/    # DI composition root
│
├── SpreadsheetUtility.UI.Web/          # Blazor Server app (Presentation)
├── SpreadsheetUtility.UI.Console/      # Console app (Presentation)
├── SpreadsheetUtilities.Auth.Api/      # Auth Minimal API (Presentation)
│
├── SpreadsheetUtility.Test/            # All tests (xUnit) — 71 tests
│
├── SpreadsheetUtilities.ServiceDefaults/  # Aspire service defaults
└── SpreadsheetUtilities.AppHost/          # Aspire orchestrator
```

### Test Project Structure (Phase 6)

```
SpreadsheetUtility.Test/                  # Single xUnit test project
├── ApplicationTests/                     # Handler + validator unit tests
│   ├── Validators/                       # 7 FluentValidation test classes (21 methods)
│   ├── UseCases/                         # 6 MediatR handler test classes (12 methods)
│   └── MediatorIntegrationTests.cs       # End-to-end handler test (integration)
├── InfrastructureTests/                  # Infrastructure layer tests
│   ├── DoubleEntrySpreadsheetGeneratorTests.cs        # Unit tests
│   └── DoubleEntrySpreadsheetGeneratorIntegratedTests.cs  # Integration (Category=Integration)
└── DomainTests/                          # Domain layer tests
    └── EntityValueObjectTests.cs          # 10 entity/value object tests
```

## Project Dependencies

```
SpreadsheetUtility.Domain          net10.0     [no dependencies]
       ↑
SpreadsheetUtility.Application     net10.0     [Domain, MediatR, FluentValidation]
       ↑
SpreadsheetUtility.Infrastructure  net10.0     [Domain, Application, ClosedXML]
       ↑
SpreadsheetUtility.Bootstrapper    net10.0     [Application, Infrastructure]
       ↑
├── SpreadsheetUtility.UI.Web      net10.0     [Bootstrapper]
├── SpreadsheetUtility.UI.Console  net10.0     [Bootstrapper]
└── SpreadsheetUtilities.Auth.Api  net10.0     [Bootstrapper]

Shared:
  SpreadsheetUtilities.ServiceDefaults  net10.0  [Aspire]
  SpreadsheetUtilities.AppHost          net10.0  [Aspire orchestration]
```

## Key Folders (Application)

| Path | Description |
|------|-------------|
| `Application/DTOs/` | 9 DTOs: `TaskDto`, `ProjectDto`, `DeveloperDto`, `CalculateGanttChartAllocationInput/Output`, `DeveloperAvailability`, `ListGeneratorInput`, `GenerateDoubleEntryInput/Output` |
| `Application/Ports/` | Abstractions: `IDateTimeProvider`, `IHolidayProvider`, `IExcelWorkbook`, `IExcelWorksheet`, `IDoubleEntryGeneratorService`, `IAuthService`, `ISessionStorage` |
| `Application/Configuration/` | Enums: `SessionStorageLocation` |
| `Application/Mappers/` | `IGanttChartMapper` / `GanttChartMapper` (DTO ↔ Domain) |
| `Application/Validation/` | 7 FluentValidation validators: CalculateGanttChartAllocation, GenerateDoubleEntry, LoadTasks, ParseExcelData, InitiateSession, UpdateSession, GetSession |
| `Application/Services/` | Calculators, strategies, factories, list generators, builders, PasteParserService |
| `Application/UseCases/` | MediatR queries/commands and handlers (CalculateGanttChartAllocation, LoadTasks, ParseExcelData, Session, GenerateDoubleEntrySpreadsheet) |
| `Application/Behaviors/` | `LoggingBehavior`, `ValidationBehavior` pipelines |

## Key Folders (Infrastructure)

| Path | Description |
|------|-------------|
| `Infrastructure/Excel/` | ClosedXML implementations: `IExcelDocument`/`ExcelDocument`, `DoubleEntryGeneratorService` |
| `Infrastructure/Services/` | `AuthService`, `SessionService`, `FolderExampleFileProvider`, `AuthApiSessionStorage`, `LocalMemorySessionStorage`, `RedisSessionStorage`, `SessionStorageSelector` |
| `Infrastructure/Providers/` | `DateTimeProvider`, `HolidayFileProvider` |
| `Infrastructure/Repositories/` | `HolidayRepository`, `DeveloperRepository` |
| `Infrastructure/ApiClients/` | NSwag-generated `AuthApiClient` |
| `Infrastructure/Models/` | `SessionState`, `ExampleFileInfo`, `FileDownloadDto` |
| `Infrastructure/Abstractions/` | `IExampleFileProvider` |
| `Infrastructure/Holidays/` | `2025HolidaysPT.json` |

## Layer Responsibility

| Layer | Responsible For |
|-------|----------------|
| **Domain** | Entities (`GanttTask`, `Developer`, `Project`, `ProjectGroup`, `Holiday`), Value Objects (`DateRange`, `VacationPeriod`), Domain service interfaces, Repository interfaces |
| **Application** | Use case orchestration via MediatR, DTOs, mapping, port abstractions, business logic services (calculators, strategies, PasteParserService) |
| **Infrastructure** | All I/O (ClosedXML Excel operations), repository implementations, external API clients, caching (`IMemoryCache`), providers (date, holiday, example files) |
| **Bootstrapper** | DI composition: single `AddSpreadsheetUtilities()` extension method calling `AddApplication()` + `AddInfrastructure()` |
| **Presentation** | Blazor components/pages, Minimal API endpoints, console I/O |

## Key Files

| File | Description |
|------|-------------|
| `Application/DependencyInjection.cs` | `AddApplication()` extension for DI registration (MediatR, validators, services) |
| `Application/Behaviors/LoggingBehavior.cs` | MediatR pipeline: logs requests/responses |
| `Application/Behaviors/ValidationBehavior.cs` | MediatR pipeline: validates inputs via FluentValidation |
| `Application/UseCases/CalculateGanttChartAllocation/` | Query + Handler replacing `GanttChartProcessor` |
| `Application/UseCases/LoadTasks/` | Query + Handler for loading tasks from Excel |
| `Application/UseCases/ParseExcelData/` | Command + Handler for parsing pasted Excel data |
| `Application/UseCases/GenerateDoubleEntrySpreadsheet/` | Command + Handler for double-entry spreadsheet generation |
| `Application/UseCases/Session/` | InitiateSession, GetSession, UpdateSession use cases |
| `Application/Services/IPasteParserService.cs` | Interface for parsing pasted TSV data |
| `Application/Services/PasteParserService.cs` | Implementation: ParseProjects, ParseTasks, ParseTeam |
| `Bootstrapper/DependencyInjection.cs` | `AddSpreadsheetUtilities()` extension combining Application and Infrastructure registrations |
| `Infrastructure/DependencyInjection.cs` | `AddInfrastructure()` extension for DI registration |
| `Infrastructure/Excel/DoubleEntryGeneratorService.cs` | ClosedXML-based implementation of `IDoubleEntryGeneratorService` |
| `Infrastructure/Services/AuthService.cs` | IAuthService implementation using IMemoryCache |
| `UI.Web/ViewModels/GanttGeneratorViewModel.cs` | Page state holder for GanttGeneratorFromPaste.razor |
