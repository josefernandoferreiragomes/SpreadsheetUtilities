using SpreadsheetUtility.Library.Models;

namespace SpreadsheetUtility.Library.ListGenerators;

    public class ListGeneratorInput
    {
        public List<Project> projectInputList {  get; set; } = new List<Project>();
        public DateTime projectStartDate { get; set; }
    }

