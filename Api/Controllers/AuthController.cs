using Application.Dto.Auth;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly TokenService _tokenService;

    public AuthController(ApplicationDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {

        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        // 3. التحقق من كلمة المرور (مقارنة النص الصريح مع الـ Hash المخزن)
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHashed);

        if (!isPasswordValid)
        {
            return Unauthorized("Invalid email or password.");
        }

        // 4. توليد الـ JWT
        var token = _tokenService.CreateToken(user);

        // 5. إرجاع الـ Token للفرونت إند
        return Ok(new
        {
            Token = token,
            UserId= user.Id,
            Role = user.Role.Name,
            TenantId= user.TenantId
        });
    }
}