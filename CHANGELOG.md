# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

### Architecture

- Phase 4a refactoring: Auth.Api now uses Application + Infrastructure layers
- Phase 4b refactoring: UI.Web — replaced GanttMapperHelper static methods with IPasteParserService in Application/Services/
- Created GanttGeneratorViewModel in UI.Web/ViewModels/ as scoped DI service for page state
- GanttGeneratorFromPaste.razor now binds to ViewModel properties and uses PasteParserService
- Register PasteParserService in Application/DependencyInjection.cs
- Register GanttGeneratorViewModel in UI.Web/Program.cs
- Build: 0 errors, Tests: 26 pass, 0 failures
- Extracted inline session logic into MediatR use cases (InitiateSessionCommand, GetSessionQuery, UpdateSessionCommand)
- Added IAuthService port in Application/Ports/ with AuthService implementation in Infrastructure/Services/
- Added session DTOs (InitiateSessionRequest/Response, GetSessionRequest/Response, UpdateSessionRequest/Response)
- Removed commented-out code and stale .http stub from Auth.Api
- Auth.Api calls AddApplication() and AddInfrastructure() for DI composition
- Added Assembly.GetExecutingAssembly() and auto-discovery scanning for the Session use case handlers
- Build: 0 errors, Tests: 26 pass, 0 failures


### Fixed

- Fixed DI resolution error in `HolidayRepository` by switching from concrete `HolidayFileProvider` dependency to `IHolidayProvider` interface

### Architecture

- Phase 2 refactoring: extracted `SpreadsheetUtility.Application` project with MediatR, FluentValidation
- DTOs moved to `Application/DTOs/` (`TaskDto`, `DeveloperDto`, `ProjectDto`, I/O classes)
- Port interfaces moved to `Application/Ports/` (`IDateTimeProvider`, `IHolidayProvider`, `IExcelWorkbook`/`IExcelWorksheet`)
- Mappers moved to `Application/Mappers/` (`IGanttChartMapper`, `GanttChartMapper`)
- Calculator services moved to `Application/Services/` (`CalculatorFacade`, `DateCalculator`, `DeveloperHoursCalculator`, strategies, list generators, builders, factories)
- Replaced `LoggingInvoker`/`ILogCommand` Command pattern with MediatR pipeline `LoggingBehavior`
- Added `ValidationBehavior` pipeline with `FluentValidation`
- Deleted `IGanttChartDataManager`/`GanttChartDataManager`; consumers now use `IMediator.Send()`
- Phase 1 refactoring: extracted `SpreadsheetUtility.Domain` project with zero dependencies
- Moved pure entities to Domain: `Holiday`, `Project`, `ProjectGroup`, `GanttTask`, `Developer`
- Removed `IDateCalculator` dependency from `Developer` entity; extracted logic into `TaskAssignmentStrategyBase`
- Swapped `GanttTask` serialization from `Newtonsoft.Json` to `System.Text.Json` (built-in)
- Added value objects: `DateRange`, `VacationPeriod`
- Added domain service interfaces: `IDateCalculationService`, `IHolidayLookupService`
- Added repository interfaces: `IHolidayRepository`, `IDeveloperRepository`
- Updated all imports across Library, Tests, and Web UI to reference `SpreadsheetUtility.Domain.Models`

### Added

- opencode AI assistant configuration (AGENTS.md)

## [1.2.0] - 2026-06-04

### Architecture

- Phase 3 refactoring: created `SpreadsheetUtility.Infrastructure` project with `Microsoft.AspNetCore.App` framework reference
- Moved `DateTimeProvider` â†’ `Infrastructure/Providers/DateTimeProvider.cs`
- Moved `HolidayProvider` (renamed to `HolidayFileProvider`) â†’ `Infrastructure/Providers/HolidayFileProvider.cs`
- Moved and renamed `IExcelWorkbook`/`ExcelWorkbook` â†’ `Infrastructure/Excel/IExcelDocument`/`ExcelDocument` (avoids naming collision with `Application.Ports.IExcelWorkbook`)
- Moved `SessionService` â†’ `Infrastructure/Services/SessionService.cs`
- Moved `FolderExampleFileProvider` + `IExampleFileProvider` â†’ `Infrastructure/Services/` and `Infrastructure/Abstractions/`
- Moved NSwag-generated `AuthApiClient` â†’ `Infrastructure/ApiClients/AuthApiClient.cs`
- Moved models (`SessionState`, `ExampleFileInfo`, `FileDownloadDto`) â†’ `Infrastructure/Models/`
- Moved `2025HolidaysPT.json` â†’ `Infrastructure/Holidays/`
- Implemented `IHolidayRepository` (`HolidayRepository`) and `IDeveloperRepository` (`DeveloperRepository`) in `Infrastructure/Repositories/`
- Created `Infrastructure/DependencyInjection.cs` with `AddInfrastructure()` extension method
- `UI.Web` dropped `Library` project reference; now depends on `Infrastructure` + `Application`
- `Library` project stripped to legacy `SpreadsheetGenerator` only; references `Infrastructure` for `IExcelDocument`
- `UI.Console` and `Test` added `Infrastructure` project reference
- Updated all imports and namespaces across consuming projects
- `dotnet build`: 0 errors, `dotnet test`: 26 pass, 0 failures

## [1.1.0] - 2024-01-15

### Added

- Example Files Download feature: browse and download example .xlsx files from the Gantt Generator UI
- `IExampleFileProvider` / `FolderExampleFileProvider` service abstraction for file serving
- REST API endpoint at `/api/examplefiles` (list, download, metadata)
- `ExampleFilesDownload.razor` Blazor page with security validation (directory traversal prevention, .xlsx-only)
- `FileServerExampleFileProvider` ready for future migration
- JavaScript helper (`file-download.js`) for browser downloads
- Navigation link to Example Files page in NavMenu

### Security

- Directory traversal prevention
- File extension validation (.xlsx only)
- Null character validation

## [1.0.0] - 2024-01-01

### Added

- Session Management System with email-based authentication
- In-memory session caching with thread-safe `Dictionary<string, SessionState>`
- Encrypted cookie storage using Data Protection API (DPAPI)
- Session state auto-restore on page reload
- `SessionService` with `InitiateSession`, `UpdateSession`, `ClearSession` methods
- Gantt Generator Blazor page rewrite with conditional authentication UI
- Email validation and session initialization flow

### Security

- Email-to-GUID two-level session identification
- Base64 encoding for safe cookie transport
- `lock`-based thread-safe cache operations

### Architecture

- 11 design patterns implemented in SpreadsheetUtility.Library (Strategy, Factory, Template Method, Builder, Facade, Mapper/Adapter, Observer, Command, Dependency Injection, Provider, Generic List Generator)



