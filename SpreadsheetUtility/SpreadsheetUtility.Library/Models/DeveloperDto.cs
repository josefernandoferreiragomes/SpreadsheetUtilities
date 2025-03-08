using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtility.Library
{
    public class DeveloperDto
    {
     
        public required string Team { get; set; }
        public required string Name { get; set; }
        public required double DailyWorkHours { get; set; }
        public required string VacationPeriods { get; set; }
    }
}
