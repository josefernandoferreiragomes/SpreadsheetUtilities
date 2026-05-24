using SpreadsheetUtility.Library.Models;
using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Library.ListGenerators;
public interface IListGenerator<TInput, TOutput>
{
    List<TOutput> GenerateList(IEnumerable<TInput> input, ListGeneratorInput listGeneratorInput);
}