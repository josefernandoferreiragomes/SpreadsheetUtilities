# ETL Assistant — Project Execution Plan

## Objective

Build a Blazor page (\"ETL Assistant\") that lets users paste arbitrary tabular data in three
separate text areas (one each for projects, tasks, and team data). A local LLM
(qwen2.5-3b-instruct running at http://localhost:1234) transforms each paste into the
corresponding projects.txt, tasks.txt, or team.txt format — or explains why the ETL
is not possible.

## Deliverables

| # | File | Status |
|---|------|--------|
| 1 | docs/ETL_PROJECT_PLAN.md | This file |
| 2 | Application/Ports/ILlmTransformerService.cs | [ ] |
| 3 | Infrastructure/Options/LlmOptions.cs | [ ] |
| 4 | Infrastructure/Services/LlmTransformerService.cs | [ ] |
| 5 | UI.Web/ViewModels/EtlAssistantViewModel.cs | [ ] |
| 6 | UI.Web/wwwroot/js/file-download-etl.js | [ ] |
| 7 | UI.Web/Components/Pages/EtlAssistant.razor | [ ] |
| 8 | SpreadsheetUtility.Test/InfrastructureTests/LlmTransformerServiceTests.cs | [ ] |

## Files Modified

| # | File | Change | Status |
|---|------|--------|--------|
| 1 | UI.Web/Components/Layout/NavMenu.razor | Add nav link | [ ] |
| 2 | Infrastructure/DependencyInjection.cs | Register LlmTransformerService + options + HttpClient | [ ] |
| 3 | UI.Web/Program.cs | Register EtlAssistantViewModel | [ ] |
| 4 | UI.Web/appsettings.json | Add Llm config section | [ ] |
| 5 | CHANGELOG.md | Add unreleased entry | [ ] |
| 6 | docs/PROJECT_STRUCTURE.md | Update tables | [ ] |

## Implementation Order

1. Ports (interface + DTO + enum)
2. Options class
3. Service implementation
4. ViewModel
5. JavaScript helper
6. Blazor page
7. NavMenu link
8. DI registrations + config
9. Tests
10. Build → Test → Smoke → Review → Governance → Commit

## Target Schemas (LLM Output)

### projects.txt
ProjectID\tProject Name\tProject Group Id\tTeam Id

### tasks.txt
ID\tProject Id\tProjectName\tTaskName\tEstimatedEffortHours\tDependencies\tProgress\tInternalID

### team.txt
Team ID\tTeam Name\tDeveloper Id\tDeveloper Name\tDeveloper Vacation Date Intervals\tDaily Work Hours

## LLM Protocol

If the LLM cannot extract sufficient data for the target schema, it must prefix its
response with \"IMPOSSIBLE:|\" followed by a plain-English explanation.
