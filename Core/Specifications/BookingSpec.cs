using Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class BookingSpec : Specification<Booking>
    {
        public BookingSpec(DateTime EventDate) : base(I => I.Event.Date.Date == EventDate)
        {
            Include.Add(I => I.Event);
            Include.Add(I => I.AppUser);
        }
        public BookingSpec(string userid) : base(I => I.AppUserId == userid)
        {
            Include.Add(I => I.Event);
            Include.Add(I => I.AppUser);
        }
        public BookingSpec(string userid , int eventid):base(I=>I.AppUserId == userid && I.EventId == eventid)
        {
            Include.Add(I => I.Event);
            Include.Add(I => I.AppUser);
        }
    }
}
