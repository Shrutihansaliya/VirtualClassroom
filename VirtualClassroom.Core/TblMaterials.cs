using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualClassroom.Core
{
    //public class TblMaterials
    //{
    //    [Key]
    //    public int MaterialId { get; set; }

    //        public int ClassroomId { get; set; }

    //        public string Title { get; set; }
    //        public string Description { get; set; }

    //        public string FilePath { get; set; }
    //        public string FileType { get; set; }

    //        public int UploadedBy { get; set; }
    //        public DateTime UploadedAt { get; set; }

    //        public bool IsVisible { get; set; }

    //        // Navigation
    //        public TblClassroom Classroom { get; set; }
    //        public TblUsers Faculty { get; set; }

    //}


    public class TblMaterials
    {
        [Key]
        public int MaterialId { get; set; }

        public int ClassroomId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public string FilePath { get; set; }
        public string FileType { get; set; }

        public int UploadedBy { get; set; }

        [ForeignKey("UploadedBy")]
        public TblUsers Faculty { get; set; }

        public DateTime UploadedAt { get; set; }

        public bool IsVisible { get; set; }

        public TblClassroom Classroom { get; set; }
    }
}
