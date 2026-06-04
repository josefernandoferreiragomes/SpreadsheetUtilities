---
name: update-governance-docs
description: Update governance MD files (CHANGELOG, roadmap, project structure) after completing a phase, and capture state for session handoff
---

# Governance Docs Update Skill

## Purpose

This skill documents the three governance files that track project progress and state. Use it:
- After completing a phase (or significant sub-step)
- When handing off between sessions (context window limit, new agent, etc.)
- Before starting a new phase to load current state

## The Three Governance Files

### 1. `CHANGELOG.md` (repo root)

**Purpose:** Chronological log of all changes, organized by release.

**Update when:**
- Every feature or bugfix is completed
- After a build+test green result
- Keep entries under `## [Unreleased]` until a release is cut

**Format:**
```markdown
### Category Heading (e.g., Architecture, UI.Console, Fixed)

- Bullet point per change
- Include new files created, files modified, services registered
- Always end with: `- Build: 0 errors, Tests: XX pass, 0 failures`
```

### 2. `docs/REFACTORING_ROADMAP.md`

**Purpose:** Live checklist of the phased DDD/Clean Architecture refactoring plan. Shows what is done, what is in progress, and what remains.

**Update when:**
- A phase or sub-step transitions from 🔄/⬜ to ✅
- A step is found to be blocked or deferred
- The plan itself changes

**Format per phase:**
```markdown
### Phase N — Title

| # | Step | Status |
|---|---|---|
| **N.1** | Step description | ✅ Done |
| N.2 | Step description | ⬜ Pending |
```

Mark the phase header with ✅ when all sub-steps are complete.

### 3. `docs/PROJECT_STRUCTURE.md`

**Purpose:** Snapshot of the solution layout, project dependencies, folder organization, and key files.

**Update when:**
- A new project is added or removed from the solution
- Dependencies change between projects
- Key folders/files are added or reorganized
- The layer responsibility table changes

## Handoff Procedure

When you run out of context window or need to stop mid-phase:

1. **Save state** — Ensure all three governance files are up to date with:
   - What was completed this session
   - What is in progress (with specific next steps)
   - What is blocked (with the reason)

2. **Report to user** — Tell them:
   - What was accomplished
   - What remains (exact next step)
   - Ask them to start a new session and load this skill

3. **New session startup** — Load this skill, then read the three governance files to reconstruct full context.

## Current Project State (last updated: 2026-06-04)

### Completed Phases
- Phase 0 — Foundation & Namespace Unification ✅
- Phase 1 — Domain Layer ✅
- Phase 2 — Application Layer ✅
- Phase 3 — Infrastructure Layer ✅
- Phase 4a — UI.Web Refactoring ✅
- Phase 4b — Auth.Api Refactoring ✅
- Phase 4c — UI.Console Refactoring ✅
- **Phase 4 — Presentation Layer Refactoring (ALL)** ✅

### Next: Phase 5 — Cross-Cutting & Final Cleanup

| # | Step | Status |
|---|---|---|
| **5.1** | Delete `SpreadsheetUtility.Library` after all code migrated | ⬜ |
| **5.2** | Create `SpreadsheetUtility.Bootstrapper` extension methods or use `ServiceDefaults` patterns for DI composition | ⬜ |
| **5.3** | Update `.sln` solution folders to match architecture: `src/Domain/`, `src/Application/`, `src/Infrastructure/`, `src/Presentation/` | ⬜ |
| **5.4** | Update CHANGELOG.md, docs/, AGENTS.md | ⬜ |
| **5.5** | dotnet build — clean | ⬜ |

### After Phase 5: Phase 6 — Testing Restructure
### After Phase 6: Phase 7 — Final Cleanup & Library Deletion

### Key Constraints
- ViewModels stay in UI.Web project, not Application/DTOs
- Minimal API endpoints go in `Endpoints/` folder
- DI validation via `UseDefaultServiceProvider` (standard ASP.NET Core pattern)
- All new features need unit tests (xUnit)
- Use MediatR for use case orchestration
- Infrastructure owns all ClosedXML/NSwag references
- Console uses `Host.CreateDefaultBuilder` + `AddApplication()` + `AddInfrastructure()`
