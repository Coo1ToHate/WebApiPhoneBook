using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPhoneBook.Models;

namespace WebApiPhoneBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<IdentityRole>>> Get()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        
        [HttpPost("add/{name}")]
        public async Task<ActionResult<IdentityRole>> Post(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));

                if (result.Succeeded)
                {
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, IdentityRole model)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return BadRequest();
            }

            role.Name = model.Name;
            
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return Ok();
            }
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {

            IdentityRole role = await _roleManager.FindByIdAsync(id);
            
            if (role == null)
            {
                return NotFound();
            }

            IdentityResult result = await _roleManager.DeleteAsync(role);

            return Ok();
        }

        [HttpGet("{userId}")]
        [AllowAnonymous]
        public async Task<IList<string>> GetRoleUser(string userId)
        {
            User tmp = await _userManager.FindByIdAsync(userId);
            return await _userManager.GetRolesAsync(tmp);
        }
        
        [HttpPost("editUserRole/{userId}")]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);
                await _userManager.AddToRolesAsync(user, addedRoles);
                await _userManager.RemoveFromRolesAsync(user, removedRoles);
                return Ok();
            }
            return BadRequest();
        }
    }
}
