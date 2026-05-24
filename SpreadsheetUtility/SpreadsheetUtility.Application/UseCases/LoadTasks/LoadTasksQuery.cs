using MediatR;
using SpreadsheetUtility.Application.DTOs;
using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Application.UseCases.LoadTasks;

public record LoadTasksQuery(List<TaskDto> TaskDtos) : IRequest<List<GanttTask>>;
