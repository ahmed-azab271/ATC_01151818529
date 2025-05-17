using Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class LikeSpec : Specification<Like>
    {
        public LikeSpec(int CategoryId) : base(I => I.Event.Category.Id == CategoryId)
        {
            Include.Add(I => I.Event);
            Include.Add(I => I.AppUser);
        }
        public LikeSpec(int EventId , string AppuserId) : base(I=>(I.EventId == EventId) && (I.AppUserId == AppuserId))
        {
            Include.Add(I => I.Event);
            Include.Add(I => I.AppUser);
        }
    }
}
