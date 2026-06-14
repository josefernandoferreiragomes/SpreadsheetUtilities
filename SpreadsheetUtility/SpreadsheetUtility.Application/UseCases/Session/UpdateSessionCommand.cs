using MediatR;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Application.UseCases.Session;

public record UpdateSessionCommand(string Email, Guid SessionId, string NewValue, CacheBackend cache = CacheBackend.Memory) : IRequest<UpdateSessionResponse>;
