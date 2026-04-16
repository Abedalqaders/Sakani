using Application.Common.Interfaces.Renter;
using Application.Dto.Auth;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly TokenService _tokenService;
    private readonly IRenterRepository _renterRepository;
    public AuthController(ApplicationDbContext context, TokenService tokenService, IRenterRepository renterRepository)
    {
        _context = context;
        _tokenService = tokenService;
        _renterRepository = renterRepository;
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
        Guid renterId = Guid.Empty;
        if (user.Role.Name == "Renter") // تأكد إن الاسم هون بيطابق الاسم في الداتابيز
        {
            var renter = await _renterRepository.GetByUserIdAsync(user.Id);
            renterId = renter?.Id ?? Guid.Empty;    

            // حماية إضافية: لو الـ Role تبعه Renter بس ما إله بروفايل في جدول الـ Renters
            if (renterId == Guid.Empty)
                return Unauthorized("Renter profile not found.");
        }
        // 4. توليد الـ JWT
        var token = _tokenService.CreateToken(user, renterId);
        user.LastLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();
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