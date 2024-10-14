using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using snap_backend.Data;
using snap_backend.Models;
using snap_backend.Services;

namespace snap_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;
        private readonly JwtSettings _jwtSettings;

        public AuthController(ApplicationDbContext context, TokenService tokenService,IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
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
                Id = Guid.NewGuid(),
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password),
                Role = Role.Courier
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Courier registered successfully!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginUser)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginUser.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginUser.Password, user.Password))
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = accessToken;
            user.RefreshTokenExpiration = DateTime.Now.AddMinutes(_jwtSettings.RefreshTokenExpiration);
            await _context.SaveChangesAsync();

            return Ok(new {AccessToken = accessToken, RefreshToken = refreshToken });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefresToken([FromBody] string refreshToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiration <= DateTime.Now)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token." });
            }

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiration = DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpiration);
            await _context.SaveChangesAsync();

            return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
        }
           
    }
}
