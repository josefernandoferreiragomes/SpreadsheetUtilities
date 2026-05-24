using MediatR;
using SpreadsheetUtility.Application.DTOs;

namespace SpreadsheetUtility.Application.UseCases.ParseExcelData;

public class ParseExcelDataCommand : IRequest<CalculateGanttChartAllocationInput>
{
    public string? ProjectsData { get; set; }
    public string? TasksData { get; set; }
    public string? TeamData { get; set; }
}
