using SpreadsheetUtility.Library.Models;

namespace SpreadsheetUtility.Library.ListGenerators;
public interface IListGenerator<TInput, TOutput>
{
    List<TOutput> GenerateList(IEnumerable<TInput> input, ListGeneratorInput listGeneratorInput);
}