using APIs.DTOs;
using AutoMapper;
using Core.Entites;
using Core.Entites.Identity;
using Microsoft.AspNetCore.Identity;

namespace APIs.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Event, EventDto>()
                .ForMember(I => I.ImageUrl, O => O.MapFrom<EventImageUrlResolver>())
                .ForMember(I => I.CategoryName, O => O.MapFrom(S => S.Category.Name))
                .ForMember(I => I.Tags, O => O.MapFrom(S => S.EventTags !=null ? S.EventTags.Where(T=>T.Tag !=null)
                                                                                .Select(T=>T.Tag.Name).ToList()
                                                                                : new List<string>()));

            CreateMap<Booking, BookingDto>()
                .ForMember(I => I.EventName, O => O.MapFrom(S => S.Event.Name));
                
            CreateMap<Category , CategoryDto>().ReverseMap();

            CreateMap<Tag , TagDto>().ReverseMap();

            CreateMap<IdentityRole , ReturnedRoleDto>().ReverseMap(); 
           
        }
    }
}
