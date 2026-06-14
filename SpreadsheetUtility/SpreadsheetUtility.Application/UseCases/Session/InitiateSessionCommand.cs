using MediatR;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Application.UseCases.Session;

public record InitiateSessionCommand(string Email, Guid? sessionGuid = null, CacheBackend cache = CacheBackend.Memory) : IRequest<InitiateSessionResponse>;
