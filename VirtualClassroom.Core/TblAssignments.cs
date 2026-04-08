using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualClassroom.Core
{
    public class TblAssignments
    {
        [Key]
        public int AssignmentId { get; set; }

            public int ClassroomId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }

            public DateTime DueDate { get; set; }
            public int CreatedBy { get; set; }

        // ✅ NEW FIELDS (for file upload)
        public string FilePath { get; set; }     // path of file (PDF/Image)
        public string FileType { get; set; }     // pdf, jpg, png, docx


        // Navigation
        public TblClassroom Classroom { get; set; }
            public TblUsers Faculty { get; set; }
            public ICollection<TblSubmissions> Submissions { get; set; }
        
    }
}
