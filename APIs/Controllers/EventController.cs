using APIs.DTOs;
using APIs.ErrorHandling;
using APIs.Helpers;
using APIs.Helpers.SignalR;
using AutoMapper;
using Core.Entites;
using Core.Entites.Identity;
using Core.IRepositories;
using Core.IServices;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Net;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APIs.Controllers
{
    public class EventController : BaseController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IEventServices eventServices;
        private readonly IHubContext<NotificationHub> hub;

        public EventController(UserManager<AppUser> _userManager,IUnitOfWork _unitOfWork, IMapper _mapper 
            , IEventServices _eventServices , IHubContext<NotificationHub> _hub)
        {
            userManager = _userManager;
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            eventServices = _eventServices;
            hub = _hub;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponce) , StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IReadOnlyList<Pagination<EventDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Pagination<EventDto>>> GetEvent([FromQuery] EventSpecParams? Prams)
        {
            var Spec = new EventSpec(Prams);
            var events = await unitOfWork.Entity<Event>().GetAllIncldedWithSpec(Spec,true);
            if (!events.Any())
                return NotFound(new ApiResponce(404 , "There is no Events"));
            var SpecCount = new CountEventSpec(Prams);
            var EventCount =await unitOfWork.Entity<Event>().GetCountWithSpec(SpecCount, true);
            var mapped = mapper.Map<IReadOnlyList<Event>, IReadOnlyList<EventDto>>(events);
            var paginated = new Pagination<EventDto>
            {
                Count = EventCount,
                PageSize = Prams.PageSize,
                PageIndex = Prams.PageIndex,
                Data = mapped
            };
            return Ok(paginated);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EventDto>> AddEvent(EventDto eventDto , string EndPoint = "AddEvent")
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await eventServices.ReturnOrAddCategory(eventDto.CategoryName);

            var Tags = await eventServices.ReturnOrAddTags(eventDto.Tags);
            
            var Data = new EventWithTagsAndCategoryDto() {EventDto = eventDto ,CategoryId = category.Id, Tags = Tags , EndPointName = EndPoint };
            var count = await AddOrUpdate(Data);
            if (count == 0) 
                return BadRequest(new ApiResponce(400, "The Event Not Add"));

            await SendMessage(Data);

            return Ok(eventDto);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EventDto>>UpdateEvent(EventDto eventDto, string EndPoint = "UpdateEvent")
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await eventServices.ReturnOrAddCategory(eventDto.CategoryName);

            var Tags = await eventServices.ReturnOrAddTags(eventDto.Tags);

            var Data = new EventWithTagsAndCategoryDto() { EventDto = eventDto, CategoryId = category.Id, Tags = Tags, EndPointName = EndPoint };
            var count = await AddOrUpdate(Data);
            if (count == 0) 
                return BadRequest(new ApiResponce(400, "The Event Not Updated"));
            return Ok(eventDto);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EventDto>> DeleteEvent(int id)
        {
            var Spec = new EventSpec(id);
            var Event = await unitOfWork.Entity<Event>().GetSpacificWithSpec(Spec);
            if (Event is null) 
                return NotFound(new ApiResponce(404  , "The Event Not Found"));

            var categorySpec = new CategorySpec(Event.Category.Name);
            var category = await unitOfWork.Entity<Category>().GetSpacificWithSpec(categorySpec);
            unitOfWork.Entity<Event>().Delete(Event);
            if(category.Events.Count == 1)
                unitOfWork.Entity<Category>().Delete(category);

            var count = await unitOfWork.SaveAsync();
            if(count == 0) 
                return BadRequest(new ApiResponce(400, "The Event Not Deleted"));
            bool deleted = await SaveOrDeleteEventImage.DeleteImageAsync(Event.ImageUrl);
            if (!deleted)
                return BadRequest(new ApiResponce(400, "The Event Image not Deleted"));
            var mapped = mapper.Map<Event, EventDto>(Event);
            return Ok(mapped);

        }

        [HttpGet("Categories")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetCategories()
        {
            var Spec = new CategorySpec();
            var categries = await unitOfWork.Entity<Category>().GetAllIncldedWithSpec(Spec,true);
            if (categries is null)
                return NotFound(new ApiResponce(404, "There Is No Categories"));
            var mapped = mapper.Map<IReadOnlyList<Category>, IReadOnlyList<CategoryDto>>(categries);
            return Ok(mapped);
        }
        [HttpGet("Tags")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<Tag>>> GetTags()
        {
            var Spec = new TagsSpec();
            var Tags = await unitOfWork.Entity<Tag>().GetAllIncldedWithSpec(Spec, true);
            if (Tags is null)
                return NotFound(new ApiResponce(404, "There Is No Tags"));
            var mapped = mapper.Map<IReadOnlyList<Tag>, IReadOnlyList<TagDto>>(Tags);
            return Ok(mapped);
        }
        [HttpPost("AddOrUpdate")]
        public async Task<int> AddOrUpdate( [FromBody] EventWithTagsAndCategoryDto Data)
        {
            if(Data.EndPointName == "AddEvent")
            {
                var localImagePath = await SaveOrDeleteEventImage.SaveImageFromUrlAsync(Data.EventDto.ImageUrl);
                var AddEvent = new Event()
                {
                    Name = Data.EventDto.Name,
                    Date = Data.EventDto.Date,
                    Description = Data.EventDto.Description,
                    Venue = Data.EventDto.Venue,
                    ImageUrl = localImagePath,
                    CategoryId = Data.CategoryId,
                    Price = Data.EventDto.Price,
                    EventTags = Data.Tags.Select(I => new EventTag
                    {
                        TagId = I.Id
                    }).ToList(),
                };
                await unitOfWork.Entity<Event>().AddAsync(AddEvent);
                var count = await unitOfWork.SaveAsync();
                return count;
            }
            if(Data.EndPointName == "UpdateEvent")
            {
                var eventSpec = new EventSpec(Data.EventDto.Id);
                var Event = await unitOfWork.Entity<Event>().GetSpacificWithSpec(eventSpec);
                Event.Name = Data.EventDto.Name;
                Event.Date = Data.EventDto.Date;
                Event.Description = Data.EventDto.Description;
                Event.Venue = Data.EventDto.Venue;
                Event.ImageUrl = Data.EventDto.ImageUrl;
                Event.CategoryId = Data.CategoryId;
                Event.Price = Data.EventDto.Price;
                Event.EventTags = Data.Tags.Select(I => new EventTag
                {
                    TagId = I.Id
                }).ToList();

                unitOfWork.Entity<Event>().Update(Event);
                var count = await unitOfWork.SaveAsync();
                return count;
            }
            return 0;
        }
        [HttpPost("SendMessage")]
        public async Task SendMessage (EventWithTagsAndCategoryDto Data)
        {
            var CtegorySpec = new LikeSpec(Data.CategoryId);
            var LikedWithCategory = await unitOfWork.Entity<Like>().GetAllIncldedWithSpec(CtegorySpec);
            if (LikedWithCategory is not null)
            {
                foreach (var Like in LikedWithCategory)
                {
                    await hub.Clients.User(Like.AppUser.Id).SendAsync("ReceiveNotification", "There is New Event You Might Intersted In");
                }
            }
        }
    }
}
