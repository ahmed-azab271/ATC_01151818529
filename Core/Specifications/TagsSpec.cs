using Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class TagsSpec : Specification<Tag>
    {
        public TagsSpec():base()
        {
            Include.Add(I => I.EventTags);
        }
        public TagsSpec(string TagName) : base(I => I.Name.ToLower().Contains(TagName))
        {
            Include.Add(I => I.EventTags);
        }
        public TagsSpec(List<string> TagName):base (I=>TagName.Contains(I.Name)) 
        {
            Include.Add(I=>I.EventTags);
        }
    }
}
