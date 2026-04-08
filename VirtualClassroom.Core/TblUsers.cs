using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

public enum UserRole
{
    Faculty = 1,
    Student = 2
}

namespace VirtualClassroom.Core
{
    public class TblUsers
    {
        [Key]
        public int UserId { get; set; }

            public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }

            public string AuthProvider { get; set; }
            public string ProviderUserId { get; set; }

            public string ProfilePicture { get; set; }
        [Required]
        public UserRole Role { get; set; }

            public DateTime CreatedAt { get; set; }

            // Navigation
            public ICollection<TblClassroom> CreatedClassrooms { get; set; }
            public ICollection<TblClassroomMembers> ClassroomMembers { get; set; }
        
    }
}
