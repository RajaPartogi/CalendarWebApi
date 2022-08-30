using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalendarWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _userDbContext;

        public UsersController(UserDbContext userDbContext)
        {
            _userDbContext = userDbContext;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUsers(User user)
        {
            _userDbContext.Users.Add(user);
            await _userDbContext.SaveChangesAsync();

            return CreatedAtAction("GetUser", new User { Id = user.Id, Name = user.Name}, user);
        }
        
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _userDbContext.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userDbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _userDbContext.Entry(user).State = EntityState.Modified;

            try
            {
                await _userDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _userDbContext.Users.Add(user);
            await _userDbContext.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userDbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _userDbContext.Users.Remove(user);
            await _userDbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _userDbContext.Users.Any(e => e.Id == id);
        }
    }
}
