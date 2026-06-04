# SpreadsheetUtilities — Agent Instructions

An ASP.NET Core project with Minimal APIs and Blazor, progressively refactored toward Domain-Driven Design and Clean Architecture.

## Conventions

- Use `dotnet` CLI for all commands
- Don't write inline documentation
- Update `CHANGELOG.md` with every new feature or bugfix
- After every new feature update the `docs/` folder
- For each new feature implement a unit test in the tests folder
- Use xUnit as the test framework
- Minimal API endpoints go in `Program.cs` or separate extension files under `Endpoints/`
- Blazor components go under `Components/` (or `Pages/`/`Shared/`)
- Apply established design patterns when implementing each feature
- Progressively refactor toward Domain-Driven Design and Clean Architecture
- Before writing code, always plan first

## Governance Docs (Session Handoff)

Three files track the project's state across sessions:

| File | Purpose |
|------|---------|
| `CHANGELOG.md` | Chronological log of all changes |
| `docs/REFACTORING_ROADMAP.md` | Phase-by-phase checklist of the refactoring plan |
| `docs/PROJECT_STRUCTURE.md` | Solution layout, dependencies, folder organization |

### Session Handoff Protocol

1. **Before stopping** (context window limit, phase boundary, or blocking issue):
   - Update all three governance docs with what was done, what's in progress, and next steps
   - Report to user: accomplishments, remaining work, exact next step
   
2. **Starting a new session:**
   - Load `update-governance-docs` skill to get the handoff template
   - Read `CHANGELOG.md` (latest [Unreleased] section)
   - Read `docs/REFACTORING_ROADMAP.md` (current phase status)
   - Read `docs/PROJECT_STRUCTURE.md` (current project layout)
   - Read `docs/REFACTORING_ROADMAP.md` (full remaining plan)

3. **Between phases:**
   - Update all three governance docs
   - Tell the user a phase is complete and ask for direction to the next phase

## Available Skills

- `build-project` — Build the solution via `.opencode/skills/build-project/scripts/build.cmd`
- `update-governance-docs` — Update governance files and capture session handoff state
- `second-opinion` — Get code review from another agent

## Current Dependency Graph (as of Phase 6 complete — 71 tests)

```
SpreadsheetUtility.Domain          [no deps]
       ↑
SpreadsheetUtility.Application     [Domain, MediatR, FluentValidation]
       ↑
SpreadsheetUtility.Infrastructure  [Domain, Application, ClosedXML]
       ↑
SpreadsheetUtility.Bootstrapper    [Application, Infrastructure]
       ↑
├── UI.Web     [Bootstrapper]
├── UI.Console [Bootstrapper]
└── Auth.Api   [Bootstrapper]

Shared:
  SpreadsheetUtilities.ServiceDefaults  [Aspire]
  SpreadsheetUtilities.AppHost          [Aspire orchestration]
```

## Build & Test

```bash
dotnet build     # 0 errors expected
dotnet test      # 71/71 pass expected
```


