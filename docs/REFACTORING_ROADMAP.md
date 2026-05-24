# DDD / Clean Architecture Refactoring Roadmap

> Progressive refactoring toward Domain-Driven Design and Clean Architecture across all projects.

## Target Architecture

```
┌─────────────────────────────────────────────────────┐
│                  Presentation                        │
│  SpreadsheetUtility.UI.Web (Blazor)                  │
│  SpreadsheetUtility.UI.Console                       │
│  SpreadsheetUtilities.Auth.Api (Minimal API)         │
├─────────────────────────────────────────────────────┤
│                   Application                        │
│  Use Cases (MediatR), DTOs, Mappers, Port Interfaces │
├─────────────────────────────────────────────────────┤
│                    Domain                            │
│  Entities, Value Objects, Domain Services, Events    │
├─────────────────────────────────────────────────────┤
│                Infrastructure                        │
│  Repositories, File I/O, External API Clients,       │
│  Caching, Providers (Holiday, DateTime)              │
├─────────────────────────────────────────────────────┤
│              Shared / Cross-cutting                  │
│  SpreadsheetUtilities.ServiceDefaults (Aspire)       │
│  SpreadsheetUtilities.AppHost (orchestrator)         │
└─────────────────────────────────────────────────────┘
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

## Phase 0 — Foundation & Namespace Unification ✅

**Scope:** All projects
**Goal:** Eliminate legacy `Utilities.*` namespace before structural changes
**Status:** ✅ Complete (2026-05-23)

| # | Step | Result |
|---|---|---|
| 0.1 | Migrate `Utilities.Interfaces` → `SpreadsheetUtility.Library.Infrastructure` | ✅ `IExcelWorkbook` moved, namespace updated, `Interfaces/` folder deleted |
| 0.2 | Migrate `Utilities.Services` → `SpreadsheetUtility.Library.Infrastructure` | ✅ `ExcelWorkbook` moved, namespace updated, `Managers/ExcelWorkbook.cs` deleted |
| 0.3 | Fix `SpreasheetGeneratorBase` / `SpreasheetGeneratorDoubleEntry` typo → `Spreadsheet...` | ✅ 3 class renames + 3 file renames across Library, Console, Tests, SimplifiedConsole |
| 0.4 | Create `Directory.Build.props` for consistent analyzers, versioning, `ImplicitUsings`, `Nullable` | ✅ Created at repo root; duplicate props stripped from all 7 `.csproj` files |
| 0.5 | Delete empty `Library.DataAccess/` and `Library.Identity/` folders | ✅ Folders removed; orphaned GUID cleaned from `.sln` |
| 0.6 | `dotnet build` / `dotnet test` — green | ✅ Build: 0 errors, 110 pre-existing warnings. Tests: 19 pass, 7 pre-existing failures unchanged |

---

## Phase 1 — Domain Layer (`SpreadsheetUtility.Domain`) ✅

**Extract from `Processors/GanttChartProcessor/Domain/` and `Models/`**
**Status:** ✅ Complete (2026-05-24)

| # | Step | Result |
|---|---|---|
| **1.1** | Create project; **zero dependencies** (not even NuGet beyond `net10.0`) | ✅ `SpreadsheetUtility.Domain` created, added to solution |
| **1.2** | Move pure entities: `Developer`, `GanttTask`, `Project`, `ProjectGroup`, `Holiday` | ✅ All 5 entities moved to `Domain/Models/` |
| **1.3** | Extract value objects: `DateRange`, `VacationPeriod` | ✅ `DateRange` and `VacationPeriod` in `Domain/ValueObjects/` (deferred `DeveloperAvailability` and `EffortHours` to Phase 2) |
| **1.4** | **Remove `IDateCalculator` dependency from `Developer`** — extract `NextAvailableDate()` logic into a domain service | ✅ `Developer` is now constructor-free; `NextAvailableDate()` logic inlined in `TaskAssignmentStrategyBase`; domain service interface `IDateCalculationService` defined |
| **1.5** | Define domain service interfaces: `IDateCalculationService`, `IHolidayLookupService` (pure, no I/O) | ✅ Created in `Domain/Services/` |
| **1.6** | Define repository interfaces: `IHolidayRepository`, `IDeveloperRepository` (contracts only) | ✅ Created in `Domain/Repositories/` |
| 1.7 | Add `Domain` ref to Library; update imports; `dotnet test` | ✅ Build: 0 errors. Tests: 26 pass, 0 failures |

---

## Phase 2 — Application Layer (`SpreadsheetUtility.Application`) ✅

**Extract use-case orchestration from `GanttChartProcessor` and related classes**
**Status:** ✅ Complete (2026-05-24)

| # | Step | Result |
|---|---|---|
| **2.1** | Create project; references `Domain` + `MediatR` (+ `FluentValidation`) | ✅ `SpreadsheetUtility.Application` created, added to solution |
| **2.2** | Move DTOs (`TaskDto`, `ProjectDto`, `DeveloperDto`, input/output classes) → `Application/DTOs/` | ✅ 7 files moved, namespaces updated |
| **2.3** | Move `GanttChartMapper` → `Application/Mappers/` (DTO ↔ Domain) | ✅ `IGanttChartMapper` and `GanttChartMapper` moved |
| **2.4** | **Split `GanttChartProcessor` god-class into MediatR use cases:** | ✅ All 3 use cases implemented |
| | - `CalculateGanttChartAllocationQuery` / `Handler` | ✅ |
| | - `LoadTasksQuery` / `Handler` | ✅ |
| | - `ParseExcelDataCommand` / `Handler` (moved from UI helper) | ✅ |
| **2.5** | Move port interfaces: `IHolidayProvider`, `IDateTimeProvider`, `IExcelWorkbook` → `Application/Ports/` | ✅ With refactored `IExcelWorksheet` wrapper (no ClosedXML leakage) |
| **2.6** | Move `CalculatorFacade`, `DateCalculator`, `DeveloperHoursCalculator` → `Application/Services/` | ✅ All calculator services moved |
| **2.7** | Move `ListGenerators`, `Grouppers`, `Builders` → `Application/Services/` | ✅ All moved |
| **2.8** | Replace `LoggingInvoker` Command pattern with **MediatR pipeline behavior** (`LoggingBehavior<TRequest, TResponse>`) | ✅ `LoggingBehavior` created; `LoggingInvoker`/`ILogCommand` deleted |
| **2.9** | Add `ValidationBehavior` pipeline for `FluentValidation` | ✅ `ValidationBehavior` created |
| **2.10** | Move `SpreadsheetGenerator` (legacy) → `Application/UseCases/` as `GenerateSpreadsheetCommand` | 🔄 Deferred to Phase 3 |
| **2.11** | Replace `IGanttChartDataManager` with direct use case injection | ✅ `IGanttChartDataManager`/`GanttChartDataManager` deleted; UI pages use `IMediator.Send()` |
| **2.12** | `dotnet test` — green | ✅ 26 pass, 0 failures |

### Test Fix Applied

- Changed all `System.Reflection.MethodBase.GetCurrentMethod()?.Name` calls to `nameof(...)` in JSON-driven tests, because async state machine returns `MoveNext` instead of the actual method name

---

## Phase 3 — Infrastructure Layer (`SpreadsheetUtility.Infrastructure`)

**Move all I/O, external dependencies, and provider implementations**

| # | Step |
|---|---|
| **3.1** | Create project; references `Application` + `Domain` + `ClosedXML` |
| **3.2** | `HolidayProvider` → `Infrastructure/Providers/HolidayFileProvider.cs` |
| **3.3** | `DateTimeProvider` → `Infrastructure/Providers/DateTimeProvider.cs` |
| **3.4** | `ExcelWorkbook` / `IExcelWorkbook` → `Infrastructure/Excel/` |
| **3.5** | `SessionService` (moved from UI.Web) → `Infrastructure/Services/SessionService.cs` |
| **3.6** | `AuthApiClient` (NSwag) → `Infrastructure/ApiClients/` |
| **3.7** | Implement repo interfaces from Domain |
| 3.8 | Add `Holidays/2025HolidaysPT.json` as embedded resource |
| 3.9 | `dotnet test` — green |

---

## Phase 4 — Presentation Layer Refactoring

### 4a. `SpreadsheetUtility.UI.Web`

| # | Step |
|---|---|
| **4.1** | Move `GanttMapperHelper` static methods → `Application/UseCases/ParseExcelDataCommand` |
| **4.2** | **Split `GanttGeneratorFromPaste.razor`** (567 lines → components): `SessionComponent`, `GanttConfigComponent`, `GanttResultsComponent` |
| **4.3** | Create ViewModel classes for Blazor pages (stop exposing Domain entities directly) |
| **4.4** | Replace `IGanttChartDataManager` → inject `IMediator` + specific use cases |
| **4.5** | Refactor `ExampleFilesController` from `[ApiController]` to Minimal API endpoint in `Program.cs` |
| 4.6 | Remove direct `SpreadsheetUtility.Library` reference; only reference `Application` + `Infrastructure` |
| 4.7 | Add `MediatR` + `FluentValidation` services in `Program.cs` |
| 4.8 | Replace manual service validation loop with DI validation from `ServiceDefaults` |

### 4b. `SpreadsheetUtilities.Auth.Api`

| # | Step |
|---|---|
| **4.9** | Extract inline session logic → `Application/UseCases/SessionUseCase` (or `InitiateSessionCommand`, etc.) |
| **4.10** | Define proper request/response DTOs |
| **4.11** | Implement `IAuthService` in Infrastructure using `IMemoryCache` |
| 4.12 | Remove commented-out code from `Program.cs` |

### 4c. `SpreadsheetUtility.UI.Console`

| # | Step |
|---|---|
| **4.13** | Merge `SimplifiedUtilityConsole` logic into this project |
| 4.14 | Add DI setup using `Host.CreateDefaultBuilder` |
| 4.15 | Wire up Infrastructure + Application + Domain |
| 4.16 | Remove `SimplifiedUtilityConsole` project from solution |

---

## Phase 5 — Cross-Cutting & Final Cleanup

| # | Step |
|---|---|
| **5.1** | **Delete `SpreadsheetUtility.Library`** after all code migrated |
| **5.2** | Create `SpreadsheetUtility.Bootstrapper` extension methods or use `ServiceDefaults` patterns for DI composition |
| **5.3** | Update `.sln` solution folders to match architecture: `src/Domain/`, `src/Application/`, `src/Infrastructure/`, `src/Presentation/` |
| **5.4** | Update `CHANGELOG.md`, `docs/`, `AGENTS.md` |
| **5.5** | `dotnet build` — clean |

---

## Phase 6 — Testing Restructure

| # | Step |
|---|---|
| 6.1 | Split tests into folders: `Domain.Tests/`, `Application.Tests/`, `Infrastructure.Tests/` (in existing test project or separate projects) |
| 6.2 | Add MediatR handler unit tests for each use case |
| 6.3 | Add `FluentValidation` tests for input DTOs |
| 6.4 | Move integration tests touching filesystem → `Infrastructure.Tests` |
| 6.5 | `dotnet test --collect:"XPlat Code Coverage"` — verify coverage maintained |

---

## Final Dependency Graph

```
SpreadsheetUtility.Domain          net10.0     [no dependencies]
       ↑
SpreadsheetUtility.Application     net10.0     [Domain, MediatR, FluentValidation]
       ↑
┌──────┴──────────────────┐
Infrastructure           Presentation
[Domain, Application,    ├─ UI.Web    [Application, Infrastructure]
 ClosedXML, NSwag]       ├─ UI.Console[Application, Infrastructure]
                         └─ Auth.Api  [Application, Infrastructure]

Shared:
  ServiceDefaults (Aspire) — unchanged
  AppHost (Aspire orchestration) — unchanged
```

## Solution Folder Layout (target)

```
src/
├── SpreadsheetUtility.Domain/
├── SpreadsheetUtility.Application/
├── SpreadsheetUtility.Infrastructure/
└── Presentation/
    ├── SpreadsheetUtility.UI.Web/
    ├── SpreadsheetUtility.UI.Console/
    └── SpreadsheetUtilities.Auth.Api/

tests/
└── SpreadsheetUtility.Test/

shared/
├── SpreadsheetUtilities.ServiceDefaults/
└── SpreadsheetUtilities.AppHost/
```
