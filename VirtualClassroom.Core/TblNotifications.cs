using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VirtualClassroom.Core
{
    public class TblNotifications
    {
        [Key]
        public int NotificationId { get; set; }

            public int UserId { get; set; }

            public string Title { get; set; }
            public string Message { get; set; }

            public bool IsRead { get; set; }
            public DateTime CreatedAt { get; set; }

            // Navigation
            public TblUsers User { get; set; }
        
    }
}
