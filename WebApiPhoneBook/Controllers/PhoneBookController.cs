using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPhoneBook.Data;
using WebApiPhoneBook.Models;

namespace WebApiPhoneBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhoneBookController : ControllerBase
    {
        private readonly DataContext _context;

        public PhoneBookController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhoneBook>>> Get()
        {
            return await _context.PhoneBooks.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PhoneBook>> Get(int id)
        {
            var phoneBook = await _context.PhoneBooks.FindAsync(id);

            if (phoneBook == null)
            {
                return NotFound();
            }

            return phoneBook;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PhoneBook>> Post(PhoneBook phoneBook)
        {
            _context.PhoneBooks.Add(phoneBook);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = phoneBook.Id }, phoneBook);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Put(int id, PhoneBook phoneBook)
        {
            if (id != phoneBook.Id)
            {
                return BadRequest();
            }

            _context.Entry(phoneBook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhoneBookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var phoneBook = await _context.PhoneBooks.FindAsync(id);
            if (phoneBook == null)
            {
                return NotFound();
            }

            _context.PhoneBooks.Remove(phoneBook);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhoneBookExists(int id)
        {
            return _context.PhoneBooks.Any(e => e.Id == id);
        }
    }
}
