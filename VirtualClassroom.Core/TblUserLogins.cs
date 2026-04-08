using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VirtualClassroom.Core
{
    public class TblUserLogins
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string Provider { get; set; }
        [Required]
        public string ProviderUserId { get; set; }

            // Navigation
            public TblUsers User { get; set; }
        
    }
}
