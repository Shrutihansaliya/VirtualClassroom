using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VirtualClassroom.Core
{
    public class TblSubmissions
    {
        [Key]
        public int SubmissionId { get; set; }

            public int AssignmentId { get; set; }
            public int StudentId { get; set; }

            public string FilePath { get; set; }
            public DateTime SubmittedAt { get; set; }

            public int? Marks { get; set; }

            // Navigation
            public TblAssignments Assignment { get; set; }
            public TblUsers Student { get; set; }
        
    }
}
