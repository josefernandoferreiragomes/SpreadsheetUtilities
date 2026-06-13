# Changelog

## [Unreleased]

### Added

- Session Admin page: Added "Session List Source" selector card with dropdown and "Get" button to load sessions from a chosen storage location independently of the save location
- SessionStorageSelector.GetStorage(SessionStorageLocation) — new public method resolving any location to its ISessionStorage implementation
- SessionService.FetchSessionsFromLocationAsync(SessionStorageLocation) — fetches sessions from a specified storage location
- Tests: 74 pass, 0 failures

### Changed

- Session Admin page "Storage Location" selector now only affects where new sessions are saved (no longer reloads the session list)
- Session Admin page initial load uses FetchSessionsFromLocationAsync with the current storage location
- Session Admin page: SessionData field now renders with real newlines and spaces
  instead of JSON-escaped \t \n sequences for readability
- SessionService now depends on SessionStorageSelector instead of direct HttpClient/SpreadsheetUtilitiesAuthApiClient

### Added

- Session Storage Location Selector feature with runtime-switchable backends:
  - New ISessionStorage port interface in Application/Ports
  - New SessionStorageLocation enum in Application/Configuration
  - AuthApiSessionStorage — delegates to Auth.Api via NSwag client
  - LocalMemorySessionStorage — local IMemoryCache (same pattern as AuthService)
  - RedisSessionStorage — stub (throws NotImplementedException)
  - SessionStorageSelector — runtime resolver that caches the current location choice in IMemoryCache
- SessionService refactored to use SessionStorageSelector instead of creating HttpClient directly
- Session Admin page (/admin/sessions) now has a storage location selector card above the session table
- Session Admin page injects SessionStorageSelector and allows switching between Auth API, UI.Web memory, and Redis (stub) at runtime
- Tests: 74 pass, 0 failures

### Fixed

- AuthService._sessionIndex moved from instance field to IMemoryCache (key __SessionIndex)
  so GetAllSessions() works across HTTP requests — previously the index was lost
  because AuthService is registered as scoped and each request got a new empty index

- AuthService.InitiateSession() now stores an initial value under the session GUID key in IMemoryCache,
  so GetAllSessions() correctly returns SessionData for freshly created sessions (previously always null)
- AuthService.InitiateSession() now rejects duplicate emails with InvalidOperationException
  instead of silently overwriting the existing session
- SessionAdmin.razor now calls Auth.Api via HTTP (SessionService.FetchAllSessionsFromApiAsync())
  instead of using local IMediator/IAuthService (UI.Web and Auth.Api are separate
  processes with separate caches - only HTTP calls reach the real session store)
- SessionInfoDto converted from mutable class to ecord with { get; init; } properties
  for consistency with other session DTOs
- ListSessionsAsync() added to SpreadsheetUtilitiesAuthApiClient NSwag partial class
  to call Auth.Api's /listSessions endpoint via HTTP

### Code Analysis

- Phase 7 warning cleanup: eliminated all 22 code analysis diagnostics (20 CA1848 + 1 CS8625 + 1 WHITESPACE)
- Fixed CA1848: Replaced 20 ILogger.Log*() calls with [LoggerMessage] source-generated delegates across 3 files:
  - LoggingBehavior.cs (2 instances) - added partial to class for source generator
  - FolderExampleFileProvider.cs (7 instances) - added partial to class
  - ExampleFilesEndpoints.cs (11 instances) - added static partial to class
- Fixed CS8625: Changed IAuthService.GetSession() return type from string to string? to properly model nullable session
- Fixed WHITESPACE: Auto-formatted DoubleEntrySpreadsheetGeneratorSimplified.cs with dotnet format
- Moved NSwag #pragma warning disable from auto-generated AuthApiClient.cs to project-level <NoWarn> in Infrastructure .csproj
- Removed all #pragma warning directives from AuthApiClient.cs (now covered by project-level suppressions)
- Build: 0 errors, 0 warnings. dotnet format --verify-no-changes: clean.
- Tests: 71 pass, 0 failures.


### Fixed

- **Bug: Stale ViewModel state on navigation** — GanttGeneratorFromPaste page now calls ViewModel.Reset() in OnInitializedAsync(), clearing all session-related state (email, session ID, project/task/team data) when the user navigates to the page. Previously the Scoped ViewModel retained stale data across page navigations.
- **Bug: Session data lost after storage location switch** — GetSessionState now deserializes the combined JSON from SessionData to restore all three data fields (ProjectData, TaskData, TeamData) when hydrating from the backend. SaveProjectData/SaveTaskData/SaveTeamData now persist a combined JSON snapshot of all three fields, preventing data overwrites. Legacy plain-string data is supported via fallback.
- Created CombinedSessionData record in Infrastructure/Models/ to store all three data fields as a single JSON object
- Build: 0 errors, Tests: 74 pass, 0 failures
### Architecture

- Phase 6 testing restructure: reorganized test project into ApplicationTests/, InfrastructureTests/, DomainTests/ folders
- Created 7 FluentValidation validators for all MediatR commands/queries (activating previously dormant validation pipeline)
- Added 7 validator test classes (21 new test methods)
- Added 6 MediatR handler unit test classes (12 new test methods)
- Added domain entity/value object test class (10 new test methods)
- Tagged integration tests with Category=Integration trait for CI filtering
- Added Domain project reference to test project
- Added FluentValidation NuGet package to test project (for FluentValidation.TestHelper)
- Removed empty IntegratedTests/ placeholder folder
- Build: 0 errors, Tests: 71 pass, 0 failures
- Test count grew from 26 to 71 (+173% coverage increase)

- Phase 5 refactoring: deleted SpreadsheetUtility.Library project after migrating all code
- Created SpreadsheetUtility.Bootstrapper project with AddSpreadsheetUtilities() extension method
- Refactored tests: migrated DoubleEntrySpreadsheetGenerator tests from Library to DoubleEntryGeneratorService
- All presentation projects (UI.Web, UI.Console, Auth.Api) now reference Bootstrapper instead of Application + Infrastructure directly
- Restructured solution virtual folders: src/Domain, src/Application, src/Infrastructure, src/Presentation, tests, shared
- Updated all governance docs
- Build: 0 errors, Tests: 26 pass, 0 failures

- Phase 4a refactoring: Auth.Api now uses Application + Infrastructure layers
- Phase 4b refactoring: UI.Web replaced GanttMapperHelper static methods with IPasteParserService in Application/Services/
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

### UI.Web Component Split & Cleanup

- Split 535-line GanttGeneratorFromPaste.razor into 4 components: SessionComponent, DataPasteGridComponent, GanttConfigComponent, GanttResultsComponent
- Converted ExampleFilesController ([ApiController]) to Minimal API endpoints in Endpoints/
- Replaced manual DI validation loop with UseDefaultServiceProvider.ValidateOnBuild/ValidateScopes
- Updated _Imports.razor with shared namespaces (Application.Services, ViewModels, QuickGrid, Newtonsoft.Json)
- Build: 0 errors, Tests: 26 pass, 0 failures

### UI.Console Phase 4c Refactoring

- Created GenerateDoubleEntryInput/Output DTOs in Application/DTOs
- Created IDoubleEntryGeneratorService port in Application/Ports
- Created DoubleEntryGeneratorService implementation in Infrastructure/Excel (consolidates logic from SimplifiedUtilityConsole and Library)
- Created GenerateDoubleEntrySpreadsheetCommand + Handler in Application/UseCases/
- Registered DoubleEntryGeneratorService in Infrastructure/DependencyInjection.cs
- Rewrote SpreadsheetUtility.UI.Console/Program.cs with Host.CreateDefaultBuilder DI + IMediator
- Console now references Application + Infrastructure; removed Library reference
- Removed SimplifiedUtilityConsole project from solution
- Build: 0 errors, Tests: 26 pass, 0 failures

### Fixed

- AuthService._sessionIndex moved from instance field to IMemoryCache (key __SessionIndex)
  so GetAllSessions() works across HTTP requests previously the index was lost
  because AuthService is registered as scoped and each request got a new empty index

- Fixed DI resolution error in HolidayRepository by switching from concrete HolidayFileProvider dependency to IHolidayProvider interface


### Fixed

- **Bug: Stale ViewModel state on navigation** — GanttGeneratorFromPaste page now calls ViewModel.Reset() in OnInitializedAsync(), clearing all session-related state (email, session ID, project/task/team data) when the user navigates to the page. Previously the Scoped ViewModel retained stale data across page navigations.
- **Bug: Session data lost after storage location switch** — GetSessionState now deserializes the combined JSON from SessionData to restore all three data fields (ProjectData, TaskData, TeamData) when hydrating from the backend. SaveProjectData/SaveTaskData/SaveTeamData now persist a combined JSON snapshot of all three fields, preventing data overwrites. Legacy plain-string data is supported via fallback.
- Created CombinedSessionData record in Infrastructure/Models/ to store all three data fields as a single JSON object
- Build: 0 errors, Tests: 74 pass, 0 failures
### Architecture

- Phase 2 refactoring: extracted SpreadsheetUtility.Application project with MediatR, FluentValidation
- DTOs moved to Application/DTOs/ (TaskDto, DeveloperDto, ProjectDto, I/O classes)
- Port interfaces moved to Application/Ports/ (IDateTimeProvider, IHolidayProvider, IExcelWorkbook/IExcelWorksheet)
- Mappers moved to Application/Mappers/ (IGanttChartMapper, GanttChartMapper)
- Calculator services moved to Application/Services/ (CalculatorFacade, DateCalculator, DeveloperHoursCalculator, strategies, list generators, builders, factories)
- Replaced LoggingInvoker/ILogCommand Command pattern with MediatR pipeline LoggingBehavior
- Added ValidationBehavior pipeline with FluentValidation
- Deleted IGanttChartDataManager/GanttChartDataManager; consumers now use IMediator.Send()
- Phase 1 refactoring: extracted SpreadsheetUtility.Domain project with zero dependencies
- Moved pure entities to Domain: Holiday, Project, ProjectGroup, GanttTask, Developer
- Removed IDateCalculator dependency from Developer entity; extracted logic into TaskAssignmentStrategyBase
- Swapped GanttTask serialization from Newtonsoft.Json to System.Text.Json (built-in)
- Added value objects: DateRange, VacationPeriod
- Added domain service interfaces: IDateCalculationService, IHolidayLookupService
- Added repository interfaces: IHolidayRepository, IDeveloperRepository
- Updated all imports across Library, Tests, and Web UI to reference SpreadsheetUtility.Domain.Models

### Changed

- Session Admin page: SessionData field now renders with real newlines and spaces
  instead of JSON-escaped \t \n sequences for readability

### Added

- opencode AI assistant configuration (AGENTS.md)

## [1.2.0] - 2026-06-04
- Moved NSwag-generated AuthApiClient to Infrastructure/ApiClients/AuthApiClient.cs
- Moved models (SessionState, ExampleFileInfo, FileDownloadDto) to Infrastructure/Models/
- Moved 2025HolidaysPT.json to Infrastructure/Holidays/
- Implemented IHolidayRepository (HolidayRepository) and IDeveloperRepository (DeveloperRepository) in Infrastructure/Repositories/
- Created Infrastructure/DependencyInjection.cs with AddInfrastructure() extension method
- UI.Web dropped Library project reference; now depends on Infrastructure + Application
- Library project stripped to legacy SpreadsheetGenerator only; references Infrastructure for IExcelDocument
- UI.Console and Test added Infrastructure project reference
- Updated all imports and namespaces across consuming projects
- dotnet build: 0 errors, dotnet test: 26 pass, 0 failures

## [1.1.0] - 2024-01-15

### Changed

- Session Admin page: SessionData field now renders with real newlines and spaces
  instead of JSON-escaped \t \n sequences for readability

### Added

- Example Files Download feature: browse and download example .xlsx files from the Gantt Generator UI
- IExampleFileProvider / FolderExampleFileProvider service abstraction for file serving
- REST API endpoint at /api/examplefiles (list, download, metadata)
- ExampleFilesDownload.razor Blazor page with security validation (directory traversal prevention, .xlsx-only)
- FileServerExampleFileProvider ready for future migration
- JavaScript helper (file-download.js) for browser downloads
- Navigation link to Example Files page in NavMenu

### Security

- Directory traversal prevention
- File extension validation (.xlsx only)
- Null character validation

## [1.0.0] - 2024-01-01

### Changed

- Session Admin page: SessionData field now renders with real newlines and spaces
  instead of JSON-escaped \t \n sequences for readability

### Added

- Session Management System with email-based authentication
- In-memory session caching with thread-safe Dictionary<string, SessionState>
- Encrypted cookie storage using Data Protection API (DPAPI)
- Session state auto-restore on page reload
- SessionService with InitiateSession, UpdateSession, ClearSession methods
- Gantt Generator Blazor page rewrite with conditional authentication UI
- Email validation and session initialization flow

### Security

- Email-to-GUID two-level session identification
- Base64 encoding for safe cookie transport
- lock-based thread-safe cache operations


### Fixed

- **Bug: Stale ViewModel state on navigation** — GanttGeneratorFromPaste page now calls ViewModel.Reset() in OnInitializedAsync(), clearing all session-related state (email, session ID, project/task/team data) when the user navigates to the page. Previously the Scoped ViewModel retained stale data across page navigations.
- **Bug: Session data lost after storage location switch** — GetSessionState now deserializes the combined JSON from SessionData to restore all three data fields (ProjectData, TaskData, TeamData) when hydrating from the backend. SaveProjectData/SaveTaskData/SaveTeamData now persist a combined JSON snapshot of all three fields, preventing data overwrites. Legacy plain-string data is supported via fallback.
- Created CombinedSessionData record in Infrastructure/Models/ to store all three data fields as a single JSON object
- Build: 0 errors, Tests: 74 pass, 0 failures
### Architecture

- 11 design patterns implemented in SpreadsheetUtility.Library (Strategy, Factory, Template Method, Builder, Facade, Mapper/Adapter, Observer, Command, Dependency Injection, Provider, Generic List Generator)


