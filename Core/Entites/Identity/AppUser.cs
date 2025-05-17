using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites.Identity
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}
