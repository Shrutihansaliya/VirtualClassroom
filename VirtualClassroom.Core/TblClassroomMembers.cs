using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualClassroom.Core
{
    public class TblClassroomMembers
    {
        
            public int Id { get; set; }

            public int ClassroomId { get; set; }
            public int UserId { get; set; }

        public string Email { get; set; } // ✅ ADD THIS

        public DateTime JoinedAt { get; set; }

            // Navigation
            public TblClassroom Classroom { get; set; }
            public TblUsers User { get; set; }
        
    }
}
