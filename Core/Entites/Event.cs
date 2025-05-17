using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites
{
    public class Event : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Venue { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }


        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
        public ICollection<EventTag> EventTags { get; set; } = new HashSet<EventTag>();
        public ICollection<Like> Likes { get; set; } = new HashSet<Like>();
    }
}
