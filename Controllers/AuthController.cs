using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using snap_backend.Data;
using snap_backend.Models;

namespace snap_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto newUser)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == newUser.Email);

            if (existingUser != null) 
            {
                return BadRequest($"User already exist {existingUser}");
            }

            var user = new User
            {
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password),
                Role = Role.Courier
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new {message = "Courier registered successfully!" });
        }
    }
}
