<#
.SYNOPSIS
    Copies opencode skills, agents, subagents, and AGENTS.md to another repo.

.DESCRIPTION
    Copies the entire .opencode/ directory (excluding node_modules and package files)
    and AGENTS.md from this repo's opencode setup to a target directory.
    All occurrences of "SpreadsheetUtility" and "SpreadsheetUtilities" in the copied
    files are replaced with the target directory name, so the template is ready
    for the new project with minimal manual cleanup.

.PARAMETER TargetDir
    Path to the destination repository directory (required).

.PARAMETER SourceDir
    Path to the source repository directory. Defaults to the directory where this
    script lives.

.EXAMPLE
    powershell .\copy-opencode-template.ps1 -TargetDir "C:\Projects\MyNewApp"

    Copies the template files and renames everything to "MyNewApp".

.EXAMPLE
    powershell .\copy-opencode-template.ps1 -TargetDir "C:\Projects\MyNewApp" -SourceDir "C:\Users\me\template-repo"

    Copies from a specific source repo instead of the script's directory.
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$TargetDir,

    [Parameter(Mandatory = $false)]
    [string]$SourceDir
)

if (-not $SourceDir) {
    $SourceDir = Split-Path -Parent $PSCommandPath
}

$SourceOpenCode = Join-Path $SourceDir '.opencode'
$SourceAgents = Join-Path $SourceDir 'AGENTS.md'
$TargetOpenCode = Join-Path $TargetDir '.opencode'
$TargetAgents = Join-Path $TargetDir 'AGENTS.md'

$repoName = Split-Path -Leaf $TargetDir

if (-not (Test-Path $SourceOpenCode)) {
    Write-Host "ERROR: Source .opencode directory not found at $SourceOpenCode"
    exit 1
}

Write-Host "=== Copying opencode template to $TargetDir ==="
Write-Host "  Repo name for substitution: $repoName"

# Create target directories
$null = New-Item -ItemType Directory -Path $TargetOpenCode -Force

# Copy .opencode/ top-level items, skipping node_modules and package files
$excludeNames = @('node_modules', 'package.json', 'package-lock.json')
Write-Host "  Copying .opencode/ ..."
Get-ChildItem -Path $SourceOpenCode | ForEach-Object {
    if ($_.Name -in $excludeNames) { return }
    $targetPath = Join-Path $TargetOpenCode $_.Name
    if ($_.PSIsContainer) {
        Copy-Item -LiteralPath $_.FullName -Destination $targetPath -Recurse -Force
    } else {
        Copy-Item -LiteralPath $_.FullName -Destination $targetPath -Force
    }
}

# Copy AGENTS.md
if (Test-Path $SourceAgents) {
    Write-Host "  Copying AGENTS.md ..."
    Copy-Item -LiteralPath $SourceAgents -Destination $TargetAgents -Force
} else {
    Write-Host "  WARNING: AGENTS.md not found at source, skipping"
}

# Replace project/solution name with repo name in copied text files
Write-Host "  Replacing project names with '$repoName' ..."
Get-ChildItem -Path $TargetDir -Recurse -Include '*.md', '*.json', '*.cmd', '*.ps1', '*.gitignore' | ForEach-Object {
    $content = Get-Content -LiteralPath $_.FullName -Raw -ErrorAction SilentlyContinue
    if ($content) {
        $modified = $content -replace 'SpreadsheetUtilities', $repoName
        $modified = $modified -replace 'SpreadsheetUtility', $repoName
        if ($modified -ne $content) {
            Set-Content -LiteralPath $_.FullName -Value $modified -NoNewline
        }
    }
}

Write-Host ""
Write-Host "=== Copy complete ==="
Write-Host ""
Write-Host "Files copied to: $TargetDir"
Write-Host ""
Write-Host "=== Manual customization checklist ==="
Write-Host "  [ ] Ports in $TargetOpenCode\skills\smoke-test\SKILL.md and smoke-test.ps1"
Write-Host "  [ ] Test count in $TargetAgents"
Write-Host "  [ ] Project structure / dependency graph in $TargetAgents"
Write-Host "  [ ] Project paths in $TargetOpenCode\skills\smoke-test\scripts\smoke-test.ps1"
Write-Host "  [ ] Phase/status label in $TargetAgents"
Write-Host "  [ ] $TargetOpenCode\opencode.json (MCP configs)"
Write-Host "  [ ] $TargetOpenCode\skills\update-governance-docs\SKILL.md (project-specific constraints)"
