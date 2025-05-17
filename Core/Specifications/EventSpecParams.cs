using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class EventSpecParams
    {
        public string? Category { get; set; }
        public string? Venue { get; set; }
        public string? TagName { get; set; }
        public decimal? Price { get; set; }
        public int PageIndex { get; set; } = 1;

        private int pagesize = 5;
        public int PageSize
        {
            get { return pagesize; }
            set { pagesize = value > 10 ? 10 : value; }
        }

    }
}
