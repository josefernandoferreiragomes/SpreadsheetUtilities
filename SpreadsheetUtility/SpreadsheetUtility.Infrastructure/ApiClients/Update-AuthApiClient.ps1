$ProjectDir = $PSScriptRoot
$SpecPath = Join-Path $ProjectDir "SpreadsheetUtilities.Auth.json"
$OutputPath = Join-Path $ProjectDir "ApiClients\AuthApiClient.cs"
$OutputDir = Split-Path -Path $OutputPath -Parent

# Create output directory if it doesn't exist
if (-not (Test-Path $OutputDir)) {
    Write-Host "Creating output directory: $OutputDir"
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

Write-Host "Generating REST client proxy..."
Write-Host "  Spec: $SpecPath"
Write-Host "  Output: $OutputPath"

# Use the correct syntax for NSwag v14.7.1
nswag openapi2csclient /input:$SpecPath /output:$OutputPath /namespace:SpreadsheetUtility.Infrastructure.ApiClients /classname:SpreadsheetUtilitiesAuthApiClient

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Client proxy generated successfully!" -ForegroundColor Green
    Write-Host "✓ File location: $OutputPath" -ForegroundColor Green
} else {
    Write-Error "Client proxy generation failed with exit code: $LASTEXITCODE"
    exit 1
}