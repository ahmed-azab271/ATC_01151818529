using Core.Entites;
using Core.IRepositories;
using Core.IServices;
using Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class EventServices : IEventServices
    {
        private readonly IUnitOfWork unitOfWork;

        public EventServices(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public async Task<IReadOnlyList<Tag>> ReturnOrAddTags(List<string> TagName)
        {
            var tagSpec = new TagsSpec(TagName);
            var Tags = await unitOfWork.Entity<Tag>().GetAllIncldedWithSpec(tagSpec,true);
            if (Tags.Count != TagName.Count)
            {
                var DbTagNames = Tags.Select(t => t.Name.ToLower()).ToList();
                var DtoTagNames = TagName.Select(t => t.ToLower()).ToList();

                var NotFoundTags = DtoTagNames.Except(DbTagNames).ToList();
                foreach (var tag in NotFoundTags)
                {
                    var NewTag = new Tag { Name = tag };
                    await unitOfWork.Entity<Tag>().AddAsync(NewTag);
                }
                await unitOfWork.SaveAsync();
                var NewTags = await unitOfWork.Entity<Tag>().GetAllIncldedWithSpec(tagSpec);
                return NewTags;
            }
            return Tags;
        }

        public async Task<Category> ReturnOrAddCategory(string CategoryName)
        {
            var Spec = new CategorySpec(CategoryName);
            var category = await unitOfWork.Entity<Category>().GetSpacificWithSpec(Spec, true);
            if (category is null)
            {
                category = new Category() { Name = CategoryName };
                await unitOfWork.Entity<Category>().AddAsync(category);
                await unitOfWork.SaveAsync();
                var NewCategory = await unitOfWork.Entity<Category>().GetSpacificWithSpec(Spec, true);
                return NewCategory;
            }
            return category;
        }
    }
}
