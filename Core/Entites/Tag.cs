using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<EventTag> EventTags { get; set; } = new HashSet<EventTag>();
    }
}
