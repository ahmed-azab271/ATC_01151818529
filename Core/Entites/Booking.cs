using Core.Entites.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites
{
    public class Booking : BaseEntity
    {
        public int EventId { get; set; }
        public Event Event { get; set; }
        public DateTime BookedAt { get; set; } = DateTime.UtcNow;
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
