using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtility.Library
{
    public class Project
    {
        public string? ProjectID { get; set; }
        public string? ProjectName { get; set; }        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<GanttTask>? Tasks { get; set; }
        public double TotalEstimatedEffortHours { get; set; }
        public string? ProjectGroup { get; set; }
    }
}
