using APIs.DTOs;
using APIs.ErrorHandling;
using APIs.Helpers;
using AutoMapper;
using Core.Entites;
using Core.Entites.Identity;
using Core.IRepositories;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIs.Controllers
{
    public class LikeController : BaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AppUser> userManager;
        private readonly IMapper mapper;

        public LikeController(IUnitOfWork _unitOfWork , UserManager<AppUser> _userManager , IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            userManager = _userManager;
            mapper = _mapper;
        }

        [HttpPost]
        [Authorize]
        [CacheAttribute(60)]
        [ProducesResponseType(typeof(bool) , StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce),StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> Like(int Eventid)
        {
            var Spec = new EventSpec(Eventid);
            var returnedEvent = await unitOfWork.Entity<Event>().GetSpacificWithSpec(Spec , true);
            if (returnedEvent is null)
                return NotFound(new ApiResponce(404));
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(userEmail);
            var Likespec = new LikeSpec(Eventid, user.Id);
            var ifExist = await unitOfWork.Entity<Like>().GetSpacificWithSpec(Likespec , true);
            if (ifExist is not null) return BadRequest(new ApiResponce(400, "You Are Already Liked This Event"));
            var Likes = new Like()
            {
                EventId = returnedEvent.Id,
                AppUserId = user.Id
            };
            await unitOfWork.Entity<Like>().AddAsync(Likes);
            var count = await unitOfWork.SaveAsync();
            if (count == 0)
                return BadRequest(new ApiResponce(400));
            return Ok(true);

        }
        [Authorize]
        [HttpPost("Dislike")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> Dislike(int Eventid)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(userEmail);
            var spec = new LikeSpec(Eventid, user.Id);
            var LikedEvent = await unitOfWork.Entity<Like>().GetSpacificWithSpec(spec);
            if (LikedEvent == null)
                return NotFound(new ApiResponce(404));
            unitOfWork.Entity<Like>().Delete(LikedEvent);
            var count = await unitOfWork.SaveAsync();
            if (count == 0)
                return BadRequest(new ApiResponce(400));
            return Ok(true);
        }
    }
}
