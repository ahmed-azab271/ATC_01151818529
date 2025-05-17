using APIs.DTOs;
using APIs.ErrorHandling;
using AutoMapper;
using Core.Entites.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIs.Controllers
{
    public class RoleController : BaseController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;

        public RoleController(UserManager<AppUser> _userManager,RoleManager<IdentityRole> _roleManager 
            ,IMapper _mapper)
        {
            userManager = _userManager;
            roleManager = _roleManager;
            mapper = _mapper;
        }

        [HttpPost("AddRole")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ReturnedRoleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReturnedRoleDto>> AddRole (string RoleName)
        {
            var ifExist = await roleManager.FindByNameAsync(RoleName);
            if (ifExist != null)
                return BadRequest(new ApiResponce(400, "This Role Is Already Exist"));
            var role = new IdentityRole()
            {
                 Name= RoleName,
            };
            var result = await roleManager.CreateAsync(role);
            if(!result.Succeeded)
                return BadRequest(new ApiResponce(400, $"Error Happend While Creating the new Role : {RoleName}"));
            var returnedDto = mapper.Map<IdentityRole , ReturnedRoleDto>(role);
            return Ok(returnedDto);
        }

        [HttpDelete("DeleteRole")]
        [ProducesResponseType(200)]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteRole(string RoleName)
        {
            var ifExist = await roleManager.FindByNameAsync(RoleName);
            if (ifExist == null)
                return NotFound(new ApiResponce(404, "This Role Is Already Deleted"));
            var result = await roleManager.DeleteAsync(ifExist);
            if (!result.Succeeded)
                return BadRequest(new ApiResponce(400, $"Error Happend While Deleteing the Role : {RoleName}"));
            return Ok($"The Role : {RoleName} Deleted Succesfully");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponce), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<string>>> GetAllRoles()
        {
            var users = await userManager.Users.ToListAsync();
            if (!users.Any())
                return NotFound(new ApiResponce(404, "There are no Users"));
            var roles = new List<string>();
            foreach (var user in users)
            {
                var userRole = await userManager.GetRolesAsync(user);
                roles.Add(userRole.FirstOrDefault() ?? "User");
            }
            return Ok(roles);
        }
    }
}
