namespace SpreadsheetUtility.Application.Services;

public interface ICalculatorFacade
{
    IDateCalculator DateCalculator { get; }
    IDeveloperHoursCalculator DeveloperHoursCalculator { get; }
}
