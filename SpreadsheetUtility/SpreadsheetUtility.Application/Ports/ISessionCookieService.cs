namespace SpreadsheetUtility.Application.Ports;

public interface ISessionCookieService
{
    string Protect(string plaintext);
    string Unprotect(string ciphertext);
}
