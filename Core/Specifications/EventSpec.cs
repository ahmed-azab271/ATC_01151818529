using Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class EventSpec :Specification<Event>
    {
      
        public EventSpec(EventSpecParams? specParams):base(P=>
        (string.IsNullOrEmpty(specParams.Category) || P.Category.Name.ToLower().Contains(specParams.Category))
        &&
        (string.IsNullOrEmpty(specParams.Venue) || P.Venue.ToLower().Contains(specParams.Venue))
        &&
        (string.IsNullOrEmpty(specParams.TagName) || P.EventTags.Any(I=>I.Tag.Name.ToLower().Contains(specParams.TagName)))
        &&
        (specParams.Price.Equals(null) || P.Price == specParams.Price)
        )
        {
            Include.Add(P => P.Category);
            Include.Add(P => P.EventTags);
            Include.Add(P => P.Bookings);
            AddInclude("EventTags.Tag");

            ApplyPagination(specParams.PageSize*(specParams.PageIndex-1) , specParams.PageSize);
        }
        public EventSpec(int id) :base(I=>I.Id == id) 
        {
            Include.Add(P => P.Category);
            Include.Add(P => P.EventTags);
            Include.Add(P => P.Bookings);
        }
    }
}
