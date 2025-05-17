using Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.IServices
{
    public interface IEventServices
    {
        Task<IReadOnlyList<Tag>> ReturnOrAddTags(List<string> TagName);
        Task<Category> ReturnOrAddCategory(string CategoryName);
    }
}
