using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualClassroom.Core
{
    public class TblClassroomMembers
    {
        [Key]
        public int Id { get; set; }

            public int ClassroomId { get; set; }
            public int UserId { get; set; }

        public DateTime JoinedAt { get; set; }

            // Navigation
            public TblClassroom Classroom { get; set; }
            public TblUsers User { get; set; }
        
    }
}
