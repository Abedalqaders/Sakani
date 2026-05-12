using Application.Common.Interfaces.Renter;
using Application.Dto.Auth;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
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

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHashed);

            if (!isPasswordValid)
            {
                return Unauthorized("Invalid email or password.");
            }

            Guid renterId = Guid.Empty;
            if (user.Role.Name == "Renter")
            {
                var renter = await _renterRepository.GetByUserIdAsync(user.Id);
                renterId = renter?.Id ?? Guid.Empty;

                if (renterId == Guid.Empty)
                {
                    return Unauthorized("Renter profile not found.");
                }
            }
            var token = _tokenService.CreateToken(user, renterId);
           



            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Token = token,
                UserId = user.Id,
                Role = user.Role.Name,
                TenantId = user.TenantId
            });
        }

        [HttpPost("register-renter/{renterId}")]
        [Authorize(Roles = "Tenant")]
        public async Task<IActionResult> RegisterRenterAccount(Guid renterId, [FromBody] CreateUserAccountRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Email is already in use.");
            }

            var renter = await _context.Renters.FirstOrDefaultAsync(r => r.Id == renterId);
            if (renter == null)
            {
                return NotFound("Renter profile does not exist.");
            }

            if (renter.UserId != null)
            {
                return BadRequest("This renter already has a user account linked.");
            }

            var renterRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Renter");
            if (renterRole == null)
            {
                return StatusCode(500, "Renter role is not configured in the database.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                PasswordHashed = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = renterRole.Id,
                TenantId = renter.TenantId
            };

            _context.Users.Add(user);

            renter.UserId = user.Id;
            _context.Renters.Update(renter);

            await _context.SaveChangesAsync();

            return Ok(new { UserId = user.Id, Message = "Renter account created successfully." });
        }

        [HttpPost("register-tenant/{tenantId}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> RegisterTenantAccount(Guid tenantId, [FromBody] CreateUserAccountRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("User email is already in use.");
            }

            bool tenantExists = await _context.Tenants.AnyAsync(t => t.Id == tenantId);
            if (!tenantExists)
            {
                return NotFound("Tenant profile does not exist.");
            }

            var tenantRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Tenant");
            if (tenantRole == null)
            {
                return StatusCode(500, "Tenant role is not configured in the database.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                PasswordHashed = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = tenantRole.Id,
                TenantId = tenantId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { UserId = user.Id, Message = "Tenant account created and linked successfully." });
        }
    }
}