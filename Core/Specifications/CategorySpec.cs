using Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class CategorySpec : Specification<Category>
    {
        public CategorySpec()
        {
            Include.Add(I => I.Events);
        }
        public CategorySpec(string name):base(I=>I.Name == name)
        {
            Include.Add(I => I.Events);
        }
    }
}
