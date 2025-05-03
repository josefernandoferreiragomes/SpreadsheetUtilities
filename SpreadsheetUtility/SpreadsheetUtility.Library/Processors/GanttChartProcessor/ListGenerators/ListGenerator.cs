using SpreadsheetUtility.Library.Models;

namespace SpreadsheetUtility.Library.ListGenerators;
public abstract class ListGenerator<TInput, TOutput> : IListGenerator<TInput, TOutput>
    where TInput : class
    where TOutput : class
{
    internal List<Project> _projectInputList = new List<Project>();
    internal DateTime _projectStartDate = DateTime.MinValue;
    
    public List<TOutput> GenerateList(IEnumerable<TInput> input, ListGeneratorInput listGeneratorInput)
    {
        _projectInputList = listGeneratorInput.projectInputList;
        _projectStartDate = listGeneratorInput.projectStartDate;
        
        if (input.Count()==0) return new List<TOutput>();

        return input
            .GroupBy(GetGroupKey)
            .Select(g => GenerateItem(g.Key, g))
            .ToList();
    }

    protected abstract string GetGroupKey(TInput item);
    protected abstract TOutput GenerateItem(string groupKey, IEnumerable<TInput> items);
}