using APIs.DTOs;
using APIs.ErrorHandling;
using AutoMapper;
using Core.Entites.Identity;
using Core.IRepositories;
using Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Repository;
using StackExchange.Redis;

namespace APIs.Controllers
{
    public class AccountController : BaseController
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly ITokenService tokenService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public AccountController(UserManager<AppUser> _userManager , SignInManager<AppUser> _signInManager,
            ITokenService _tokenService , IUnitOfWork _unitOfWork , IMapper _mapper)
        {
            signInManager = _signInManager;
            userManager = _userManager;
            tokenService = _tokenService;
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        [HttpPost("Register")]
        [ProducesResponseType(typeof(ReturnedRegisterDto) , StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce) , StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReturnedRegisterDto>> Register(RegisterDto register)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);
            
            var EmailExist = await userManager.FindByEmailAsync(register.Email);
            
            if (EmailExist is not null)
                return BadRequest(new ApiResponce(400, $"This Email : {register.Email} Already Taken"));
           
            var userAcc = new AppUser()
            {
                FullName = register.FullName,
                Email = register.Email,
                UserName = register.Email.Split('@')[0],
            };
            var result = await userManager.CreateAsync(userAcc , register.Password);
            
            if(!result.Succeeded) 
                return BadRequest(new ApiResponce(400, "Error Happend While Creating the Account"));
            
            if(userManager.Users.Count() == 1)
                await userManager.AddToRoleAsync(userAcc , "Admin");

            var returnedDto = new ReturnedRegisterDto()
            {
                Id = userAcc.Id,
                Email = register.Email,
                Token = await tokenService.CreateTokenAsync(userAcc, userManager)
            };
            return Ok(returnedDto); 

        }
        [HttpPost("Login")]
        [ProducesResponseType(typeof(ReturnedRegisterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReturnedRegisterDto>> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) 
                return BadRequest(new ApiResponce(400 , "Wrong Email"));
            var result = await signInManager.CheckPasswordSignInAsync(user , loginDto.Password, false);
            if(!result.Succeeded) return BadRequest(new ApiResponce(400, "Wrong Password"));
            var returnedDto = new ReturnedRegisterDto()
            {
                Id = user.Id,
                Email = user.Email,
                Token = await tokenService.CreateTokenAsync(user, userManager)
            };
            return Ok(returnedDto);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteAccount")]
        [ProducesResponseType(typeof(DeletedAccountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DeletedAccountDto>> DeleteAccount (string Id)
        {
            var user = await userManager.FindByIdAsync(Id);
            if(user == null) 
                return NotFound(new ApiResponce(400 , "This Email is Already Deleted"));
            var result = await userManager.DeleteAsync(user);
            if(!result.Succeeded)
                return BadRequest(new ApiResponce(400, "Error Happend While Deleting the Email"));
            var returneddto = new DeletedAccountDto()
            {
                FullName = user.FullName,
                Email = user.Email
            };
            return Ok(returneddto);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppUserDto>> GetAllUsers()
        {
            var users = await userManager.Users.ToListAsync();
            if (!users.Any())
                return NotFound(new ApiResponce(404, "There are no Users"));
            var ReturnedUsers = new List<AppUserDto>();
            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                var Mapped = new AppUserDto()
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "User"
                };
                ReturnedUsers.Add(Mapped);
            }
            return Ok(ReturnedUsers);
        }
    }
}
