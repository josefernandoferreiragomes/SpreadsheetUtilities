# DDD / Clean Architecture Refactoring Roadmap

> Progressive refactoring toward Domain-Driven Design and Clean Architecture across all projects.

## Target Architecture

```
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚                  Presentation                        â”‚
â”‚  SpreadsheetUtility.UI.Web (Blazor)                  â”‚
â”‚  SpreadsheetUtility.UI.Console                       â”‚
â”‚  SpreadsheetUtilities.Auth.Api (Minimal API)         â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚                   Application                        â”‚
â”‚  Use Cases (MediatR), DTOs, Mappers, Port Interfaces â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚                    Domain                            â”‚
â”‚  Entities, Value Objects, Domain Services, Events    â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚                Infrastructure                        â”‚
â”‚  Repositories, File I/O, External API Clients,       â”‚
â”‚  Caching, Providers (Holiday, DateTime)              â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚              Shared / Cross-cutting                  â”‚
â”‚  SpreadsheetUtilities.ServiceDefaults (Aspire)       â”‚
â”‚  SpreadsheetUtilities.AppHost (orchestrator)         â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
```

### Design Decisions

| Decision | Choice |
|---|---|
| Project structure | Separate projects for Domain, Application, Infrastructure |
| Legacy `Utilities.*` | Refactored into new layers |
| `Library.DataAccess` / `Library.Identity` | Deleted (empty placeholders) |
| Use case framework | MediatR with pipeline behaviors |
| `SimplifiedUtilityConsole` | Merged into `SpreadsheetUtility.UI.Console` |

---

## Phase 0 â€” Foundation & Namespace Unification âœ…

**Scope:** All projects
**Goal:** Eliminate legacy `Utilities.*` namespace before structural changes
**Status:** âœ… Complete (2026-05-23)

| # | Step | Result |
|---|---|---|
| 0.1 | Migrate `Utilities.Interfaces` â†’ `SpreadsheetUtility.Library.Infrastructure` | âœ… `IExcelWorkbook` moved, namespace updated, `Interfaces/` folder deleted |
| 0.2 | Migrate `Utilities.Services` â†’ `SpreadsheetUtility.Library.Infrastructure` | âœ… `ExcelWorkbook` moved, namespace updated, `Managers/ExcelWorkbook.cs` deleted |
| 0.3 | Fix `SpreasheetGeneratorBase` / `SpreasheetGeneratorDoubleEntry` typo â†’ `Spreadsheet...` | âœ… 3 class renames + 3 file renames across Library, Console, Tests, SimplifiedConsole |
| 0.4 | Create `Directory.Build.props` for consistent analyzers, versioning, `ImplicitUsings`, `Nullable` | âœ… Created at repo root; duplicate props stripped from all 7 `.csproj` files |
| 0.5 | Delete empty `Library.DataAccess/` and `Library.Identity/` folders | âœ… Folders removed; orphaned GUID cleaned from `.sln` |
| 0.6 | `dotnet build` / `dotnet test` â€” green | âœ… Build: 0 errors, 110 pre-existing warnings. Tests: 19 pass, 7 pre-existing failures unchanged |

---

## Phase 1 â€” Domain Layer (`SpreadsheetUtility.Domain`) âœ…

**Extract from `Processors/GanttChartProcessor/Domain/` and `Models/`**
**Status:** âœ… Complete (2026-05-24)

| # | Step | Result |
|---|---|---|
| **1.1** | Create project; **zero dependencies** (not even NuGet beyond `net10.0`) | âœ… `SpreadsheetUtility.Domain` created, added to solution |
| **1.2** | Move pure entities: `Developer`, `GanttTask`, `Project`, `ProjectGroup`, `Holiday` | âœ… All 5 entities moved to `Domain/Models/` |
| **1.3** | Extract value objects: `DateRange`, `VacationPeriod` | âœ… `DateRange` and `VacationPeriod` in `Domain/ValueObjects/` (deferred `DeveloperAvailability` and `EffortHours` to Phase 2) |
| **1.4** | **Remove `IDateCalculator` dependency from `Developer`** â€” extract `NextAvailableDate()` logic into a domain service | âœ… `Developer` is now constructor-free; `NextAvailableDate()` logic inlined in `TaskAssignmentStrategyBase`; domain service interface `IDateCalculationService` defined |
| **1.5** | Define domain service interfaces: `IDateCalculationService`, `IHolidayLookupService` (pure, no I/O) | âœ… Created in `Domain/Services/` |
| **1.6** | Define repository interfaces: `IHolidayRepository`, `IDeveloperRepository` (contracts only) | âœ… Created in `Domain/Repositories/` |
| 1.7 | Add `Domain` ref to Library; update imports; `dotnet test` | âœ… Build: 0 errors. Tests: 26 pass, 0 failures |

---

## Phase 2 â€” Application Layer (`SpreadsheetUtility.Application`) âœ…

**Extract use-case orchestration from `GanttChartProcessor` and related classes**
**Status:** âœ… Complete (2026-05-24)

| # | Step | Result |
|---|---|---|
| **2.1** | Create project; references `Domain` + `MediatR` (+ `FluentValidation`) | âœ… `SpreadsheetUtility.Application` created, added to solution |
| **2.2** | Move DTOs (`TaskDto`, `ProjectDto`, `DeveloperDto`, input/output classes) â†’ `Application/DTOs/` | âœ… 7 files moved, namespaces updated |
| **2.3** | Move `GanttChartMapper` â†’ `Application/Mappers/` (DTO â†” Domain) | âœ… `IGanttChartMapper` and `GanttChartMapper` moved |
| **2.4** | **Split `GanttChartProcessor` god-class into MediatR use cases:** | âœ… All 3 use cases implemented |
| | - `CalculateGanttChartAllocationQuery` / `Handler` | âœ… |
| | - `LoadTasksQuery` / `Handler` | âœ… |
| | - `ParseExcelDataCommand` / `Handler` (moved from UI helper) | âœ… |
| **2.5** | Move port interfaces: `IHolidayProvider`, `IDateTimeProvider`, `IExcelWorkbook` â†’ `Application/Ports/` | âœ… With refactored `IExcelWorksheet` wrapper (no ClosedXML leakage) |
| **2.6** | Move `CalculatorFacade`, `DateCalculator`, `DeveloperHoursCalculator` â†’ `Application/Services/` | âœ… All calculator services moved |
| **2.7** | Move `ListGenerators`, `Grouppers`, `Builders` â†’ `Application/Services/` | âœ… All moved |
| **2.8** | Replace `LoggingInvoker` Command pattern with **MediatR pipeline behavior** (`LoggingBehavior<TRequest, TResponse>`) | âœ… `LoggingBehavior` created; `LoggingInvoker`/`ILogCommand` deleted |
| **2.9** | Add `ValidationBehavior` pipeline for `FluentValidation` | âœ… `ValidationBehavior` created |
| **2.10** | Move `SpreadsheetGenerator` (legacy) â†’ `Application/UseCases/` as `GenerateSpreadsheetCommand` | ðŸ”„ Deferred to Phase 3 |
| **2.11** | Replace `IGanttChartDataManager` with direct use case injection | âœ… `IGanttChartDataManager`/`GanttChartDataManager` deleted; UI pages use `IMediator.Send()` |
| **2.12** | `dotnet test` â€” green | âœ… 26 pass, 0 failures |

### Test Fix Applied

- Changed all `System.Reflection.MethodBase.GetCurrentMethod()?.Name` calls to `nameof(...)` in JSON-driven tests, because async state machine returns `MoveNext` instead of the actual method name

---

## Phase 3 â€” Infrastructure Layer (`SpreadsheetUtility.Infrastructure`) âœ…

**Move all I/O, external dependencies, and provider implementations**
**Status:** âœ… Complete (2026-06-04)

| # | Step | Result |
|---|---|---|
| **3.1** | Create project with `Microsoft.NET.Sdk` + `<FrameworkReference Include="Microsoft.AspNetCore.App" />`; references `Application` + `Domain` + `ClosedXML` | âœ… Created, added to solution |
| **3.2** | `HolidayProvider` â†’ `Infrastructure/Providers/HolidayFileProvider.cs` | âœ… Renamed to `HolidayFileProvider` |
| **3.3** | `DateTimeProvider` â†’ `Infrastructure/Providers/DateTimeProvider.cs` | âœ… Moved, namespace updated |
| **3.4** | `IExcelWorkbook`/`ExcelWorkbook` â†’ `Infrastructure/Excel/` as `IExcelDocument`/`ExcelDocument` (renamed to avoid collision with `Application.Ports.IExcelWorkbook`) | âœ… Moved + renamed, all consumers updated |
| **3.5** | `SessionService` (moved from UI.Web) â†’ `Infrastructure/Services/SessionService.cs` | âœ… Moved, uses `IDataProtectionProvider` from ASP.NET Core |
| **3.6** | `AuthApiClient` (NSwag) â†’ `Infrastructure/ApiClients/` | âœ… NSwag config/scripts copied, old assets cleaned from UI.Web |
| **3.7** | Implement repo interfaces from Domain: `HolidayRepository`, `DeveloperRepository` | âœ… Both implemented in `Infrastructure/Repositories/` |
| 3.8 | Move `2025HolidaysPT.json` â†’ `Infrastructure/Holidays/`; keep as `<Content CopyToOutputDirectory>` (not embedded resource) | âœ… Moved from Library; `IHolidayRepository` delegates to `HolidayFileProvider` which reads via `IConfiguration` |
| 3.9 | Move `FolderExampleFileProvider` + `IExampleFileProvider` + models from UI.Web | âœ… `IExampleFileProvider` â†’ `Abstractions/`; `FolderExampleFileProvider` â†’ `Services/`; `SessionState`, `ExampleFileInfo`, `FileDownloadDto` â†’ `Models/` |
| **3.10** | Create `DependencyInjection.cs` with `AddInfrastructure()` extension method | âœ… Registers all services at once |
| **3.11** | Update `UI.Web`: drop Library ref, add Infrastructure ref, replace individual DI with `AddInfrastructure()` | âœ… Program.cs cleaned up |
| **3.12** | Update `Library`: remove moved files/packages, add Infrastructure ref; `SpreadsheetGenerator` now uses `IExcelDocument` from Infrastructure | âœ… Library stripped to legacy generators only |
| **3.13** | Update `UI.Console` + `Test`: add Infrastructure ref, update imports | âœ… All namespaces updated |
| **3.14** | `dotnet build` / `dotnet test` â€” green | âœ… Build: 0 errors, 26 tests pass |

---

## Phase 4 â€” Presentation Layer Refactoring

### 4a. `SpreadsheetUtility.UI.Web`

| # | Step | Status |
|---|---|---|
| **4.1** | Move `GanttMapperHelper` static methods â†’ `Application/UseCases/ParseExcelDataCommand` | |
| **4.2** | **Split `GanttGeneratorFromPaste.razor`** (567 lines â†’ components): `SessionComponent`, `GanttConfigComponent`, `GanttResultsComponent` | |
| **4.3** | Create ViewModel classes for Blazor pages (stop exposing Domain entities directly) | |
| **4.4** | Replace `IGanttChartDataManager` â†’ inject `IMediator` + specific use cases | âœ… done in Phase 2 |
| **4.5** | Refactor `ExampleFilesController` from `[ApiController]` to Minimal API endpoint in `Program.cs` | |
| 4.6 | Remove direct `SpreadsheetUtility.Library` reference; only reference `Application` + `Infrastructure` | |
| **4.7** | Add `MediatR` + `FluentValidation` services in `Program.cs` | âœ… done in Phase 2 via `AddApplication()` |
| 4.8 | Replace manual service validation loop with DI validation from `ServiceDefaults` | |

### 4b. `SpreadsheetUtilities.Auth.Api`

| # | Step | Status |
|---|---|---|
| **4.9** | Extract inline session logic into MediatR use cases (`InitiateSessionCommand`, `GetSessionQuery`, `UpdateSessionCommand`) | Done |
| **4.10** | Define proper request/response DTOs | Done |
| **4.11** | Implement `IAuthService` in Infrastructure using `IMemoryCache` | Done |
| 4.12 | Remove commented-out code from `Program.cs` | Done |

### 4c. `SpreadsheetUtility.UI.Console`

| # | Step | Status |
|---|---|---|
| **4.13** | Merge `SimplifiedUtilityConsole` logic into this project | |
| 4.14 | Add DI setup using `Host.CreateDefaultBuilder` | |
| 4.15 | Wire up Infrastructure + Application + Domain | |
| 4.16 | Remove `SimplifiedUtilityConsole` project from solution |

---

## Phase 5 â€” Cross-Cutting & Final Cleanup

| # | Step | Status |
|---|---|---|
| **5.1** | **Delete `SpreadsheetUtility.Library`** after all code migrated | |
| **5.2** | Create `SpreadsheetUtility.Bootstrapper` extension methods or use `ServiceDefaults` patterns for DI composition | |
| **5.3** | Update `.sln` solution folders to match architecture: `src/Domain/`, `src/Application/`, `src/Infrastructure/`, `src/Presentation/` | |
| **5.4** | Update `CHANGELOG.md`, `docs/`, `AGENTS.md` | ðŸ”„ ongoing â€” CHANGELOG and REFACTORING_ROADMAP updated; USAGE_GUIDE and README pending |
| **5.5** | `dotnet build` â€” clean | |

---

## Phase 6 â€” Testing Restructure

| # | Step | Status |
|---|---|---|
| 6.1 | Split tests into folders: `Domain.Tests/`, `Application.Tests/`, `Infrastructure.Tests/` (in existing test project or separate projects) | |
| 6.2 | Add MediatR handler unit tests for each use case | |
| 6.3 | Add `FluentValidation` tests for input DTOs | |
| 6.4 | Move integration tests touching filesystem â†’ `Infrastructure.Tests` | |
| 6.5 | `dotnet test --collect:"XPlat Code Coverage"` â€” verify coverage maintained | |

---

## Final Dependency Graph

```
SpreadsheetUtility.Domain          net10.0     [no dependencies]
       â†‘
SpreadsheetUtility.Application     net10.0     [Domain, MediatR, FluentValidation]
       â†‘
â”Œâ”€â”€â”€â”€â”€â”€â”´â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
Infrastructure           Presentation
[Domain, Application,    â”œâ”€ UI.Web    [Application, Infrastructure]
 ClosedXML, NSwag]       â”œâ”€ UI.Console[Application, Infrastructure]
                         â””â”€ Auth.Api  [Application, Infrastructure]

Shared:
  ServiceDefaults (Aspire) â€” unchanged
  AppHost (Aspire orchestration) â€” unchanged
```

## Solution Folder Layout (target)

```
src/
â”œâ”€â”€ SpreadsheetUtility.Domain/
â”œâ”€â”€ SpreadsheetUtility.Application/
â”œâ”€â”€ SpreadsheetUtility.Infrastructure/
â””â”€â”€ Presentation/
    â”œâ”€â”€ SpreadsheetUtility.UI.Web/
    â”œâ”€â”€ SpreadsheetUtility.UI.Console/
    â””â”€â”€ SpreadsheetUtilities.Auth.Api/

tests/
â””â”€â”€ SpreadsheetUtility.Test/

shared/
â”œâ”€â”€ SpreadsheetUtilities.ServiceDefaults/
â””â”€â”€ SpreadsheetUtilities.AppHost/
```


