using SpreadsheetUtility.Application.DTOs;

namespace SpreadsheetUtility.Application.Services;

public interface IListGenerator<TInput, TOutput>
{
    List<TOutput> GenerateList(IEnumerable<TInput> input, ListGeneratorInput listGeneratorInput);
}
