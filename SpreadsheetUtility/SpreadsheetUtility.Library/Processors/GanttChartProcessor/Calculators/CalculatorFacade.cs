namespace SpreadsheetUtility.Library.Calculators;
public class CalculatorFacade : ICalculatorFacade
{
    public IDateCalculator DateCalculator { get; }
    public IDeveloperHoursCalculator DeveloperHoursCalculator { get; }

    public CalculatorFacade(IDateCalculator dateCalculator, IDeveloperHoursCalculator developerHoursCalculator)
    {
        DateCalculator = dateCalculator;
        DeveloperHoursCalculator = developerHoursCalculator;
    }
}
