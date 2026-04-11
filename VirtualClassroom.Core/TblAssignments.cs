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
        [Required]
        public int ClassroomId { get; set; }
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be 3–100 characters")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Max 500 characters allowed")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Due Date is required")]
        [DataType(DataType.DateTime)]
        public DateTime DueDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
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
