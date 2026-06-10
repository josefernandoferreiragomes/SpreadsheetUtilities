using MediatR;
using SpreadsheetUtility.Application.DTOs.Session;

namespace SpreadsheetUtility.Application.UseCases.Session;

public record InitiateSessionCommand(string Email, Guid? Guid = null) : IRequest<InitiateSessionResponse>;
