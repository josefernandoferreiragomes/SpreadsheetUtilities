﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtility.Library
{
    public class TaskDto
    {
        public required string Id { get; set; }
        public string? ProjectID { get; set; }
        public string? ProjectName { get; set; }
        public required string TaskName { get; set; }
        public required double EstimatedEffortHours { get; set; }
        public string? Dependencies { get; set; }        
        public string? Progress { get; set; }
        public string? InternalID { get; set; }

        public string? ActualStart { get; set; }
        public string? ActualEnd { get; set; }
        public int ActualProgress { get; set; }   // 0-100%
    }

}
