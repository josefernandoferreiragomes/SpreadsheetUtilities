# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- User Authentication layer with Entra ID / Email-based login
  - New project: `SpreadsheetUtility.Library.Identity`
  - User registration and password management
  - Session-based persistence
- Data Persistence Layer
  - New project: `SpreadsheetUtility.Library.DataAccess`
  - Entity Framework Core with SQL Server / SQLite support
  - Repository Pattern implementation
  - Unit of Work pattern for transaction management
- Enhanced File Handling
  - Excel/CSV file upload capability
  - Local folder monitoring for batch processing
  - File validation and error reporting
- Copilot Support Infrastructure
  - `.copilot-instructions` - Development guidelines and conventions
  - `CHANGELOG.md` - Version history tracking
  - `ARCHITECTURE.md` - Design patterns deep dive
  - `CONTRIBUTING.md` - Contribution guidelines

### Changed
- Refactored service registration in `Program.cs` using extension methods
- Improved dependency injection organization with feature-based grouping

### Deprecated
- Console-based user input workflow (clipboard paste still supported, file upload is preferred)

### Fixed
- [To be filled with specific fixes]

### Removed
- [To be filled with removed features]

### Security
- Input validation for file uploads and paste data
- Authorization checks for data access operations

---

## [1.0.0] - 2025-01-XX

### Added
- Initial release of SpreadsheetUtilities suite
- **Core Features**
  - Double Entry Spreadsheet Generator (console app)
  - Gantt Chart Generator with interactive web UI (Blazor Server)
  - Task assignment algorithms with vacation/holiday support
  - Multi-view chart visualization (Tasks, Projects, Developer Workload)

- **Architecture**
  - 11 implemented design patterns (Observer, Strategy, Factory, Template Method, Builder, Facade, Mapper/Adapter, Generic List Generator, Command, Dependency Injection, Provider)
  - Comprehensive test suite with xUnit and Moq
  - SOLID principles throughout codebase
  - Modular project structure (Library, Console UI, Web UI, Tests)

- **Library Components**
  - `GanttChartProcessor` - Main orchestrator
  - `DateCalculator` - Advanced date calculations with holiday support
  - `GanttChartMapper` - DTO to domain model transformations
  - Task assignment and sorting strategies
  - Holiday and DateTime provider abstractions

- **Web Application (Blazor Server)**
  - Paste-based Excel data import
  - QuickGrid-based data tables with sorting/filtering
  - Frappe Gantt chart visualization
  - Multiple chart modes (Week/Day view)
  - Advanced configuration options
  - Real-time developer workload analysis
  - Three primary routes:
    - `/` - Home page
    - `/ganttGeneratorFromPaste` - Interactive Gantt generator
    - `/jsonGeneratorFromPaste` - JSON data generator (experimental)

- **Console Application**
  - Command-line based double-entry spreadsheet transformation
  - ClosedXML-based Excel manipulation
  - Batch processing capability
  - Portable executable distribution

- **Documentation**
  - Comprehensive README with usage examples
  - Architecture documentation with pattern explanations
  - Code examples and contribution guidelines
  - API usage demonstrations

### Technical Stack
- **.NET 8** - Console projects and library
- **.NET 9** - Blazor Server web application
- **ClosedXML** - Excel manipulation (MIT License)
- **Newtonsoft.Json** - JSON serialization
- **Microsoft.AspNetCore.Components.QuickGrid** - Data grid UI
- **Frappe Gantt** - Chart visualization (MIT License)
- **xUnit** - Testing framework
- **Moq** - Mocking library

### Known Limitations
- Single-user session state (no persistence between sessions)
- No user authentication
- File data input requires manual paste operation
- No local file system integration

---

## Versioning Strategy

### Major Version (X.0.0)
- Breaking changes to public APIs
- New major features affecting architecture
- Significant refactoring of core components

### Minor Version (1.X.0)
- New features (backward compatible)
- New design patterns or architectural improvements
- Performance enhancements

### Patch Version (1.0.X)
- Bug fixes
- Documentation updates
- Dependency updates

---

## Release Process

1. Update version number in project files and README
2. Update CHANGELOG.md with changes
3. Create release branch: `release/vX.Y.Z`
4. Run full test suite
5. Tag commit with version: `vX.Y.Z`
6. Merge to main branch
7. Publish release notes on GitHub

---

## Migration Guides

### Upgrading from 1.0.0 to 1.1.0+
No breaking changes expected for the calculator library. New features are additive:
- File upload is optional; paste functionality remains unchanged
- Authentication is opt-in for single-org deployments
- Existing console applications work unchanged

### Upgrading from POC to Production
When moving from proof-of-concept to production:
1. Enable user authentication (see Authentication documentation)
2. Configure SQL Server database (see Database setup guide)
3. Deploy to Azure Web App with proper security settings
4. Enable application logging and monitoring

---

## Future Roadmap

### Phase 1 (Current Release)
- ✅ Design patterns implementation and documentation
- ✅ Gantt chart visualization
- ⏳ User authentication
- ⏳ Data persistence layer

### Phase 2 (Minor Updates)
- [ ] File upload enhancement
- [ ] Local folder monitoring
- [ ] Export to multiple formats
- [ ] Performance optimizations

### Phase 3 (Major Features)
- [ ] Multi-organization support
- [ ] Advanced role-based access control
- [ ] Real-time collaboration features
- [ ] REST API layer

---

## Support & Contributing

- **Issues**: Report bugs via GitHub Issues
- **Contributing**: See CONTRIBUTING.md
- **Questions**: Start a Discussion on GitHub

---

**Repository**: https://github.com/josefernandoferreiragomes/SpreadsheetUtilities
**License**: MIT
