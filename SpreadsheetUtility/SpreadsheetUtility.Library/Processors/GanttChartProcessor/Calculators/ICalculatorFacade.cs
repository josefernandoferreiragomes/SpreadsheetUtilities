namespace SpreadsheetUtility.Library.Calculators;
public interface ICalculatorFacade
{
    IDateCalculator DateCalculator { get; }
    IDeveloperHoursCalculator DeveloperHoursCalculator { get; }
}
