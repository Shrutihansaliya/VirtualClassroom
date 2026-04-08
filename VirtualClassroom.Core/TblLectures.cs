using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VirtualClassroom.Core
{
    public class TblLectures
    {
        [Key]
        public int LectureId { get; set; }

            public int ClassroomId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }

            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }

            public string LiveLink { get; set; }
            public string Platform { get; set; }
            public string Status { get; set; }

            public string RecordingUrl { get; set; }

            public int CreatedBy { get; set; }
            public DateTime CreatedAt { get; set; }

            // Navigation
            public TblClassroom Classroom { get; set; }
            public TblUsers Faculty { get; set; }
        
    }
}
