﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<Event> Events { get; set; } = new HashSet<Event>();
     
    }
}
