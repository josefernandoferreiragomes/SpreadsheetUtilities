# Usage Guide

## Quick Start — Gantt Chart Generator (Web)

```bash
# Navigate to web project
cd SpreadsheetUtility.UI.Web

# Run development server
dotnet run

# Open browser to https://localhost:5001/ganttGeneratorFromPaste
```

### Step-by-Step

1. **Prepare Your Data** — Export from Excel as tab-separated columns

   **Projects** (ProjectID, ProjectName, ProjectGroup, TeamId):
   ```
   ProjectID   ProjectName      ProjectGroup  TeamId
   P001        Website Redesign 1             TEAM-A
   P002        Mobile App       2             TEAM-B
   ```

   **Tasks** (InternalID, ProjectName, TaskName, EffortHours, Dependencies, Progress):
   ```
   InternalID  ProjectName        TaskName   EffortHours  Dependencies  Progress
   1           Website Redesign   Design     40           0             50
   2           Website Redesign   Dev        80           1             30
   3           Mobile App         Design     30           0             100
   ```

   **Team** (TeamId, Team, Name, DailyWorkHours, VacationPeriods):
   ```
   TeamId  Team    Name        DailyWorkHours  VacationPeriods
   TEAM-A  Team A  John Doe    8               2025-12-24;2025-12-26
   TEAM-A  Team A  Jane Smith  8
   TEAM-B  Team B  Bob Johnson 8               2025-01-01;2025-01-03
   ```

2. **Paste Data** — Paste each dataset into the corresponding textarea on the page

3. **Configure Options**
   - **Project Start Date** — When allocation begins
   - **Chart Mode** — Week or Day view
   - **Pre-Sort Tasks** — Enable if tasks have dependencies
   - **Team to Project Group** — Fix developers to specific project groups

4. **Generate Charts** — Click "Load Tasks Gantt Chart". Three charts render:
   - **Tasks Chart** — Individual task assignments
   - **Projects Chart** — Project-level overview
   - **Developer Tasks Chart** — Developer workload visualization

5. **Review Data Tables** — Sortable tables for projects, tasks, developers, and holidays

---

## Quick Start — Double Entry Spreadsheet Generator (Console)

```bash
cd SpreadsheetUtility.UI.Console
dotnet run input.xlsx 2 5 output.xlsx
```

**Parameters:**
- `<input.xlsx>` — Path to input file
- `<key column>` — Column index (1-based) with keys
- `<values column>` — Column index (1-based) with multi-line values
- `[output.xlsx]` (optional) — Output path
- `[headers row]` (optional) — Header row index (default: 1)
- `[worksheet index]` (optional) — Worksheet index (default: 1)

**Example transformation:**
```
Input:                         Output:
| Id | CarModel | Features               | Id | CarModel | AirConditioning | PowerSteering |
| 1  | ModelA   | AirConditioning\nPS    | 1  | ModelA   | X               | X             |
| 2  | ModelB   | PS\nBucketSeats        | 2  | ModelB   |                 | X             |
```

---

## Example Files

Browse and download example `.xlsx` files from the web UI at `/examplefiles`, or place your own files in `SpreadsheetUtility.UI.Web/ExampleFiles/`.

---

## Configuration

### Vacation Period Format
```
yyyy-MM-dd;yyyy-MM-dd|yyyy-MM-dd;yyyy-MM-dd
```
Multiple intervals are pipe-separated. Each interval is a start/end semicolon-separated pair.

### Holiday File
`Holidays/2025HolidaysPT.json` is loaded by `HolidayProvider` at startup. Format:
```json
[
  { "date": "2025-01-01", "holidayDescription": "New Year's Day" }
]
```
