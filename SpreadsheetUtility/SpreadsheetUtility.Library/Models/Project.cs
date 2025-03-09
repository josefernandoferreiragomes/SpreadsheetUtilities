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
        public string? ProjectDependency { get; set; }
    }
}
