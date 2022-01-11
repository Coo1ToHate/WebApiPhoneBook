using System.Collections.Generic;
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
        public async Task<ActionResult<IEnumerable<IdentityRole>>> Get()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        [HttpPost("{name}")]
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
    }
}
