using MediatR;
using SpreadsheetUtility.Application.DTOs.Session;

namespace SpreadsheetUtility.Application.UseCases.Session;

public record UpdateSessionCommand(string Email, Guid SessionId, string NewValue) : IRequest<UpdateSessionResponse>;
