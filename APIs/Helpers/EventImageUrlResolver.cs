using APIs.DTOs;
using AutoMapper;
using Core.Entites;

namespace APIs.Helpers
{
    public class EventImageUrlResolver : IValueResolver<Event, EventDto, string>
    {
        private readonly IConfiguration configuration;

        public EventImageUrlResolver(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public string Resolve(Event source, EventDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.ImageUrl))
                return $"{configuration["ApiBaseUrl"]}{source.ImageUrl}";
            return string.Empty;
        }
    }
}
