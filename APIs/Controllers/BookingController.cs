using APIs.DTOs;
using APIs.ErrorHandling;
using AutoMapper;
using Core.Entites;
using Core.Entites.Identity;
using Core.IRepositories;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Security.Claims;

namespace APIs.Controllers
{
    public class BookingController : BaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly UserManager<AppUser> userManager;

        public BookingController(IUnitOfWork _unitOfWork , IMapper _mapper,UserManager<AppUser> _userManager)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            userManager = _userManager;
        }
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookingDto>> BookNow (int eventid)
        {
            var Spec = new EventSpec(eventid);
            var eve = await unitOfWork.Entity<Event>().GetSpacificWithSpec(Spec,true);
            if (eve is null) 
                return NotFound(new ApiResponce(404 , $"There is no Such Event with ID : {eventid}"));
            if (eve.Date < DateTime.UtcNow)
                return BadRequest(new ApiResponce(400, "Cannot Book A Past Event"));
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(userEmail);
            var spec = new BookingSpec(user.Id,eventid);
            var booked = await unitOfWork.Entity<Booking>().GetSpacificWithSpec(spec , true);
            if (booked is not null)
                return BadRequest(new ApiResponce(400, "You Already Booked This Event"));

            var booking = new Booking()
            {
                EventId = eventid,
                AppUserId = user.Id,
            };
            await unitOfWork.Entity<Booking>().AddAsync(booking);
            var count = await unitOfWork.SaveAsync();
            if (count == 0)
                return BadRequest(new ApiResponce(400, "The Booking Not Added"));
            var bookedIncluded = await unitOfWork.Entity<Booking>().GetSpacificWithSpec(new BookingSpec(user.Id, eventid), true);
            var mapped = mapper.Map<Booking, BookingDto>(bookedIncluded);
            return Ok(mapped);
        }

        [HttpDelete]
        [Authorize]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookingDto>> CancelBooking(int eventid)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(userEmail);
            var spec = new BookingSpec(user.Id, eventid);
            var booked = await unitOfWork.Entity<Booking>().GetSpacificWithSpec(spec);
            if (booked is null)
                return NotFound(new ApiResponce(404 , "You Already Canceled This Event "));
            unitOfWork.Entity<Booking>().Delete(booked);
            var count = await unitOfWork.SaveAsync();
            if (count == 0)
                return BadRequest(new ApiResponce(400, "The Booking Not Deleted"));
            var mapped = mapper.Map<Booking, BookingDto>(booked);
            return Ok(mapped);
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(BookingDto),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookingDto>> GellAllBooked()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(userEmail);
            var Spec = new BookingSpec(user.Id);
            var booked = await unitOfWork.Entity<Booking>().GetSpacificWithSpec(Spec,true);
            if (booked is null) 
                return NotFound(new ApiResponce(404 , "There is No Booked Events"));
            var mapped = mapper.Map<Booking ,BookingDto>(booked);
            return Ok(mapped);
        }
    }
}
