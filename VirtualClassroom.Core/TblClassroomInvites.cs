using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualClassroom.Core
{
    public class TblClassroomInvites
    {
        [Key]
        public int Id { get; set; }

        public int ClassroomId { get; set; }

        public string Email { get; set; }

        public bool IsAccepted { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.Now;

        public TblClassroom Classroom { get; set; }
    }
}
