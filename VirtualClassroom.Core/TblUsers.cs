//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.ComponentModel.DataAnnotations;

//public enum UserRole
//{
//    Faculty = 1,
//    Student = 2
//}

//namespace VirtualClassroom.Core
//{
//    public class TblUsers
//    {
//        [Key]
//        public int UserId { get; set; }

//            public string FullName { get; set; }
//        [Required]
//        [EmailAddress]
//        public string Email { get; set; }
//        [Required]
//        //public string PasswordHash { get; set; }
//        public string PasswordHash { get; set; }

//        public string AuthProvider { get; set; }
//            public string ProviderUserId { get; set; }

//            public string ProfilePicture { get; set; }
//        [Required]
//        public UserRole Role { get; set; }

//            public DateTime CreatedAt { get; set; }

//            // Navigation
//            public ICollection<TblClassroom> CreatedClassrooms { get; set; }
//            public ICollection<TblClassroomMembers> ClassroomMembers { get; set; }

//    }
//}



using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualClassroom.Core
{
    public enum UserRole
    {
        Faculty = 1,
        Student = 2
    }

    [Table("TblUsers")] // map to your Azure table
    public class TblUsers
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string FullName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        // 🔐 store HASHED password here
        [Required]
        [Column("Password")] // match your DB column name
        public string PasswordHash { get; set; } = "";

        public string? AuthProvider { get; set; }

        public string? ProviderUserId { get; set; }

        public string? ProfilePicture { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation (optional)
        public ICollection<TblClassroom>? CreatedClassrooms { get; set; }

        public ICollection<TblClassroomMembers>? ClassroomMembers { get; set; }
    }
}