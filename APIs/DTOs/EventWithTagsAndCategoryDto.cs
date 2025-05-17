using Core.Entites;

namespace APIs.DTOs
{
    public class EventWithTagsAndCategoryDto
    {
        public EventDto EventDto { get; set; }
        public int CategoryId { get; set; }
        public IReadOnlyList<Tag> Tags { get; set; }
        public string EndPointName { get; set; }
    }
}
