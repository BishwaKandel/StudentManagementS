//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using StudentManagementSystemAPI.Data;
//using StudentManagementSystemAPI.Models;
//using StudentManagementSystemAPI.DTO;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using Microsoft.AspNetCore.Identity;

//namespace StudentManagementSystemAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        private readonly IPasswordHasher<Auth> _passwordHasher;
//        private readonly IConfiguration _configuration;

//        public AuthController(AppDbContext context, IPasswordHasher<Auth> passwordHasher, IConfiguration configuration)
//        {
//            _context = context;
//            _passwordHasher = passwordHasher;
//            _configuration = configuration;
//        }

//        [HttpPost("register")]
//        public IActionResult Register([FromBody] AuthDTO registerDTO)
//        {
//            if (_context.Users.Any(u => u.Username == registerDTO.Username))
//            {
//                return Conflict("Username already exists.");
//            }

//            var user = new Auth
//            {
//                Username = registerDTO.Username
//            };

//            user.PasswordHash = _passwordHasher.HashPassword(user, registerDTO.Password);

//            _context.Users.Add(user);
//            _context.SaveChanges();

//            return Ok("User registered successfully.");
//        }

//        [HttpPost("login")]
//        public IActionResult Login([FromBody] AuthDTO loginDTO)
//        {
//            var user = _context.Users.FirstOrDefault(u => u.Username == loginDTO.Username);

//            if (user == null)
//                return Unauthorized("Invalid username or password.");

//            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDTO.Password);

//            if (result != PasswordVerificationResult.Success)
//                return Unauthorized("Invalid username or password.");

//            var claims = new[]
//            {
//                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
//                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
//                new Claim("username", user.Username)
//            };

//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var token = new JwtSecurityToken(
//                issuer: _configuration["Jwt:Issuer"],
//                audience: _configuration["Jwt:Audience"],
//                claims: claims,
//                expires: DateTime.Now.AddHours(2),
//                signingCredentials: creds
//            );
//            return Ok(new
//            {
//                token = new JwtSecurityTokenHandler().WriteToken(token)
//            });
//        }
//    }
//}
