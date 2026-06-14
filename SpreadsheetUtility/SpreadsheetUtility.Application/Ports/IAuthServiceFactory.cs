namespace SpreadsheetUtility.Application.Ports;

public enum CacheBackend { Memory, Redis }

public interface IAuthServiceFactory
{
    IAuthService GetService(CacheBackend backend);
}