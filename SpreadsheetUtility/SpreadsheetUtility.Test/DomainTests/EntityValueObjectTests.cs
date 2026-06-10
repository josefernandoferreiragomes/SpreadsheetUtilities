using SpreadsheetUtility.Domain.Models;
using SpreadsheetUtility.Domain.ValueObjects;

namespace SpreadsheetUtility.Test.DomainTests;

public class EntityValueObjectTests
{
    [Fact]
    public void DateRange_Should_Create_Valid_Range()
    {
        var start = new DateTime(2025, 1, 1);
        var end = new DateTime(2025, 12, 31);
        var range = new DateRange(start, end);

        Assert.Equal(start, range.Start);
        Assert.Equal(end, range.End);
    }

    [Fact]
    public void DateRange_Should_Throw_When_Start_After_End()
    {
        Assert.Throws<ArgumentException>(() => new DateRange(new DateTime(2025, 12, 31), new DateTime(2025, 1, 1)));
    }

    [Fact]
    public void DateRange_IsWithin_Should_Return_True_For_Date_In_Range()
    {
        var range = new DateRange(new DateTime(2025, 1, 1), new DateTime(2025, 12, 31));
        Assert.True(range.IsWithin(new DateTime(2025, 6, 15)));
    }

    [Fact]
    public void VacationPeriod_Should_Create_From_DateRange()
    {
        var range = new DateRange(new DateTime(2025, 7, 1), new DateTime(2025, 7, 15));
        var period = new VacationPeriod(range);

        Assert.True(period.Range.HasValue);
        Assert.Equal(new DateTime(2025, 7, 1), period.Range.Value.Start);
        Assert.Equal(new DateTime(2025, 7, 15), period.Range.Value.End);
    }

    [Fact]
    public void VacationPeriod_IsVacation_Should_Return_True_For_Date_In_Period()
    {
        var range = new DateRange(new DateTime(2025, 7, 1), new DateTime(2025, 7, 15));
        var period = new VacationPeriod(range);

        Assert.True(period.IsVacation(new DateTime(2025, 7, 10)));
        Assert.False(period.IsVacation(new DateTime(2025, 7, 20)));
    }

    [Fact]
    public void Holiday_Should_Create_With_Required_Properties()
    {
        var date = new DateTime(2025, 12, 25);
        var holiday = new Holiday { Date = date, HolidayDescription = "Christmas" };

        Assert.Equal(date, holiday.Date);
        Assert.Equal("Christmas", holiday.HolidayDescription);
    }

    [Fact]
    public void GanttTask_Should_Create_With_Required_Properties()
    {
        var task = new GanttTask
        {
            Id = "1",
            TaskName = "Test Task"
        };

        Assert.Equal("1", task.Id);
        Assert.Equal("Test Task", task.TaskName);
        Assert.Equal(0, task.Progress);
        Assert.Equal(0, task.EffortHours);
    }

    [Fact]
    public void GanttTask_Should_Set_Properties()
    {
        var task = new GanttTask
        {
            Id = "1",
            TaskName = "Test Task",
            EffortHours = 10,
            Progress = 50,
            AssignedDeveloper = "Dev 1",
            Dependencies = "0"
        };

        Assert.Equal("1", task.Id);
        Assert.Equal("Test Task", task.TaskName);
        Assert.Equal(10, task.EffortHours);
        Assert.Equal(50, task.Progress);
        Assert.Equal("Dev 1", task.AssignedDeveloper);
        Assert.Equal("0", task.Dependencies);
    }

    [Fact]
    public void Developer_Should_Create_With_Required_Properties()
    {
        var developer = new Developer
        {
            DeveloperId = "D1",
            Name = "John Doe",
            Team = "Team Alpha",
            TeamId = "T1",
            DailyWorkHours = 8
        };

        Assert.Equal("D1", developer.DeveloperId);
        Assert.Equal("John Doe", developer.Name);
        Assert.Equal("Team Alpha", developer.Team);
        Assert.Equal("T1", developer.TeamId);
        Assert.Equal(8, developer.DailyWorkHours);
    }

    [Fact]
    public void Project_Should_Create_With_Properties()
    {
        var project = new Project
        {
            ProjectID = "P1",
            ProjectName = "Project Alpha",
            ProjectGroup = "Group 1"
        };

        Assert.Equal("P1", project.ProjectID);
        Assert.Equal("Project Alpha", project.ProjectName);
        Assert.Equal("Group 1", project.ProjectGroup);
    }

    [Fact]
    public void ProjectGroup_Should_Create_With_Properties()
    {
        var group = new ProjectGroup
        {
            ProjectGroupID = "G1",
            Projects = new List<Project>()
        };

        Assert.Equal("G1", group.ProjectGroupID);
        Assert.Empty(group.Projects);
    }
}
