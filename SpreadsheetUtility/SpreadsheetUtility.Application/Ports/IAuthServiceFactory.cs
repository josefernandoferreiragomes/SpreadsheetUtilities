namespace SpreadsheetUtility.Application.Ports;

public enum CacheBackend { Memory, Redis }

public interface IAuthServiceFactory
{
    ISessionStore GetService(CacheBackend backend);
}
