using MediatR;
using SpreadsheetUtility.Application.Mappers;
using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Application.UseCases.LoadTasks;

public class LoadTasksQueryHandler(IGanttChartMapper ganttChartMapper) : IRequestHandler<LoadTasksQuery, List<GanttTask>>
{
    public Task<List<GanttTask>> Handle(LoadTasksQuery request, CancellationToken cancellationToken)
    {
        var result = ganttChartMapper.MapGanttTasksFromTaskDtos(request.TaskDtos);
        return Task.FromResult(result);
    }
}
