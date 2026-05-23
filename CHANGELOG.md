# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

### Added

- opencode AI assistant configuration (AGENTS.md)

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
