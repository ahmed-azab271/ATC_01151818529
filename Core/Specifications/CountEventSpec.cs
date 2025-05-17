using Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class CountEventSpec : Specification<Event>
    {
        public CountEventSpec (EventSpecParams? specParams) : base(P =>
        (string.IsNullOrEmpty(specParams.Category) || P.Category.Name.ToLower().Contains(specParams.Category))
        &&
        (string.IsNullOrEmpty(specParams.Venue) || P.Venue.ToLower().Contains(specParams.Venue))
        &&
        (string.IsNullOrEmpty(specParams.TagName) || P.EventTags.Any(I => I.Tag.Name.ToLower().Contains(specParams.TagName)))
        &&
        (specParams.Price.Equals(null) || P.Price == specParams.Price)
        )
        {
           
        }
    }
}
