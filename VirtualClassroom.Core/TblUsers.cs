using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualClassroom.Core
{
    public enum UserRole
    {
        Faculty = 1,
        Student = 2
    }

   
    public class TblUsers
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string? FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Nullable for Google users
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string? PasswordHash { get; set; }

        public string? AuthProvider { get; set; }

        public string? ProviderUserId { get; set; }

        public string? ProfilePicture { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<TblClassroom>? CreatedClassrooms { get; set; }
        public ICollection<TblClassroomMembers>? ClassroomMembers { get; set; }
    }
}