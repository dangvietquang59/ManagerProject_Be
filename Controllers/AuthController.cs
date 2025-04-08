using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using ManagerProject.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // 🛠 API Đăng ký tài khoản
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (_context.Users.Any(u => u.Email == request.Email))
        {
            return BadRequest(new { message = "Email đã tồn tại!" });
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = hashedPassword,
            RoleID = 2
        };

        _context.Users.Add(user);
        int affectedRows = _context.SaveChanges();

        return Ok(new
        {
            message = "Đăng ký thành công!",
            AffectedRows = affectedRows
        });
    }

    // 🛠 API Đăng nhập
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Tìm user theo email
            var user = _context.Users
                .Where(u => u.Email == request.Email)
                .Select(u => new
                {
                    u.UserID,
                    u.Email,
                    PasswordHash = u.PasswordHash ?? "", // Tránh lỗi NULL
                    FullName = u.FullName ?? "",
                    PhoneNumber = u.PhoneNumber ?? ""
                })
                .FirstOrDefault();

            // Nếu không tìm thấy user
            if (user == null)
            {
                Console.WriteLine($"[Login Failed] Không tìm thấy user với email: {request.Email}");
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng!" });
            }

            Console.WriteLine($"[Login Attempt] User: ID={user.UserID}, Email={user.Email}");

            // Nếu PasswordHash là NULL hoặc rỗng
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                Console.WriteLine($"[Error] PasswordHash NULL hoặc rỗng cho User ID: {user.UserID}");
                return StatusCode(500, new { message = "Lỗi hệ thống: Mật khẩu không hợp lệ." });
            }

            // Kiểm tra mật khẩu
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                Console.WriteLine($"[Login Failed] Sai mật khẩu cho User ID: {user.UserID}");
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng!" });
            }

            // Tạo JWT token
            var token = GenerateJwtToken(user.Email, user.UserID);

            Console.WriteLine($"[Login Success] User ID: {user.UserID} đã đăng nhập thành công.");

            return Ok(new { token });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] Lỗi trong quá trình đăng nhập: {ex.Message}");
            return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
        }
    }

    // 🛠 API Lấy thông tin user (yêu cầu đã đăng nhập)
    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
        {
            return Unauthorized(new { message = "Không tìm thấy thông tin user." });
        }

        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            return NotFound(new { message = "User không tồn tại." });
        }

        return Ok(new { user.UserID, user.FullName, user.Email, user.RoleID });
    }

    // 🛠 Hàm tạo JWT Token
    private string GenerateJwtToken(string email, int userId)
    {
        string secretKey = _configuration["JwtSettings:Secret"]
            ?? throw new Exception("JWT Secret key is missing in configuration.");

        if (!int.TryParse(_configuration["JwtSettings:ExpirationInMinutes"], out int expirationMinutes))
        {
            expirationMinutes = 60; // Mặc định 60 phút nếu thiếu cấu hình
        }

        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

// 📌 Request Models
public class RegisterRequest
{
    [Required(ErrorMessage = "Họ và tên không được để trống.")]
    public required string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email không được để trống.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    public required string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được để trống.")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
    public required string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    [Required(ErrorMessage = "Email không được để trống.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    public required string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được để trống.")]
    public required string Password { get; set; } = string.Empty;
}
