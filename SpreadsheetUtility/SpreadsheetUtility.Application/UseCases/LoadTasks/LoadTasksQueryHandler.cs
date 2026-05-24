using MediatR;
using SpreadsheetUtility.Application.Mappers;
using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Application.UseCases.LoadTasks;

public class LoadTasksQueryHandler(IGanttChartMapper _ganttChartMapper) : IRequestHandler<LoadTasksQuery, List<GanttTask>>
{
    public Task<List<GanttTask>> Handle(LoadTasksQuery request, CancellationToken cancellationToken)
    {
        var result = _ganttChartMapper.MapGanttTasksFromTaskDtos(request.TaskDtos);
        return Task.FromResult(result);
    }
}
