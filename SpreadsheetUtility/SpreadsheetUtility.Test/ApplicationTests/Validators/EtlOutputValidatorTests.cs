using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Application.Validators;

namespace SpreadsheetUtility.Test.ApplicationTests.Validators;

public class EtlOutputValidatorTests
{
    // ── Valid outputs ─────────────────────────────────────────────

    [Fact]
    public void Validate_ValidProjects_ReturnsValid()
    {
        var output = "ProjectID\tProject Name\tProject Group Id\tTeam Id\n1\tAlpha\t1\t1\n2\tBeta\t1\t2";
        var result = EtlOutputValidator.Validate(output, TargetFormat.Projects);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_ValidTasks_ReturnsValid()
    {
        var output = "ID\tProject Id\tProjectName\tTaskName\tEstimatedEffortHours\tDependencies\tProgress\tInternalID\n1\t1\tAlpha\tDesign\t40\t\t0\tINT1";
        var result = EtlOutputValidator.Validate(output, TargetFormat.Tasks);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_ValidTeam_ReturnsValid()
    {
        var output = "Team ID\tTeam Name\tDeveloper Id\tDeveloper Name\tDeveloper Vacation Date Intervals\tDaily Work Hours\n1\tAlpha\t101\tAlice\t2026-08-10|2026-08-15\t6";
        var result = EtlOutputValidator.Validate(output, TargetFormat.Team);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    // ── Empty / whitespace ────────────────────────────────────────

    [Fact]
    public void Validate_EmptyString_ReturnsInvalid()
    {
        var result = EtlOutputValidator.Validate("", TargetFormat.Projects);
        Assert.False(result.IsValid);
        Assert.Contains("empty", result.Errors[0], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_WhitespaceOnly_ReturnsInvalid()
    {
        var result = EtlOutputValidator.Validate("   \n  \n ", TargetFormat.Projects);
        Assert.False(result.IsValid);
        Assert.Contains("empty", result.Errors[0], StringComparison.OrdinalIgnoreCase);
    }

    // ── Header errors ─────────────────────────────────────────────

    [Fact]
    public void Validate_WrongHeader_ReturnsSpecificColumnError()
    {
        var output = "ProjectID\tName\tGroupId\tTeamId\n1\tAlpha\t1\t1";
        var result = EtlOutputValidator.Validate(output, TargetFormat.Projects);
        Assert.False(result.IsValid);
        Assert.Contains("Column 2", result.Errors[0]);
        Assert.Contains("Project Name", result.Errors[0]);
        Assert.Contains("Name", result.Errors[0]);
    }

    [Fact]
    public void Validate_ExtraColumnInHeader_ReturnsError()
    {
        var output = "ProjectID\tProject Name\tProject Group Id\tTeam Id\tExtra\n1\tAlpha\t1\t1\tx";
        var result = EtlOutputValidator.Validate(output, TargetFormat.Projects);
        Assert.False(result.IsValid);
        Assert.Contains("Extra column", result.Errors[0]);
    }

    [Fact]
    public void Validate_MissingColumnInHeader_ReturnsError()
    {
        var output = "ProjectID\tProject Name\tProject Group Id\n1\tAlpha\t1";
        var result = EtlOutputValidator.Validate(output, TargetFormat.Projects);
        Assert.False(result.IsValid);
        Assert.Contains("Missing column", result.Errors[0]);
    }

    // ── Data row errors ───────────────────────────────────────────

    [Fact]
    public void Validate_WrongColumnCountInDataRow_ReturnsError()
    {
        var output = "ProjectID\tProject Name\tProject Group Id\tTeam Id\n1\tAlpha\t1";
        var result = EtlOutputValidator.Validate(output, TargetFormat.Projects);
        Assert.False(result.IsValid);
        Assert.Contains("Row 2", result.Errors[0]);
        Assert.Contains("4 columns", result.Errors[0]);
        Assert.Contains("3", result.Errors[0]);
    }

    // ── Trailing newlines / blank lines ───────────────────────────

    [Fact]
    public void Validate_TrailingBlankLines_StillValid()
    {
        var output = "ProjectID\tProject Name\tProject Group Id\tTeam Id\n1\tAlpha\t1\t1\n\n\n";
        var result = EtlOutputValidator.Validate(output, TargetFormat.Projects);
        Assert.True(result.IsValid);
    }
}
