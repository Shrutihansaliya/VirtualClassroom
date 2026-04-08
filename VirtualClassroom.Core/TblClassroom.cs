using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VirtualClassroom.Core
{
    public class TblClassroom
    {
        [Key]
        public int ClassroomId { get; set; }

            public string ClassName { get; set; }
            public string Description { get; set; }

            public int CreatedBy { get; set; }
            public DateTime CreatedAt { get; set; }

            // Navigation
            public TblUsers Faculty { get; set; }
            public ICollection<TblClassroomMembers> Members { get; set; }
            public ICollection<TblLectures> Lectures { get; set; }
            public ICollection<TblAssignments> Assignments { get; set; }
            public ICollection<TblMaterials> Materials { get; set; }
        
    }
}
