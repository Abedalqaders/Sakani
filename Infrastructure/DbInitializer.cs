using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // 1. الأدوار (Roles)
        if (!await context.Roles.AnyAsync())
        {
            var roles = new List<Role>
            {
                new Role { Id = 1, Name = "SuperAdmin" },
                new Role { Id = 2, Name = "Tenant" },
                new Role { Id = 3, Name = "Renter" }
            };
            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        // 2. إنشاء الـ SuperAdmin (أدمن النظام بالكامل)
        if (!await context.Users.AnyAsync(u => u.Email == "super@sakani.com"))
        {
            var superAdmin = new User
            {
                Id = Guid.NewGuid(),
                Name = "The Boss",
                Email = "super@sakani.com",
                PasswordHashed = BCrypt.Net.BCrypt.HashPassword("Super@123"),
                RoleId = 1, // SuperAdmin
                TenantId = null, // لا يتبع لشركة معينة
                CreatedAt = DateTime.UtcNow
            };
            await context.Users.AddAsync(superAdmin);
        }

        // 3. تعريف الشركات (Tenants)
        var tenant1Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35");
        var tenant2Id = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479");

        // الشركة الأولى: Amman Real Estate
        if (!await context.Tenants.AnyAsync(t => t.Id == tenant1Id))
        {
            await context.Tenants.AddAsync(new Tenant
            {
                Id = tenant1Id,
                Name = "Amman Real Estate Co",
                AddressCity = "Amman",
                AddressRegion = "Abdali",
                AddressStreet = "Queen Rania St",
                Email = "info@amman-re.com",
                PhoneNumber = "0791111111",
                Status = Domain.Enums.TenantStatus.Active,
                CreatedAt = DateTime.UtcNow
            });

            // مدير الشركة الأولى
            await context.Users.AddAsync(new User
            {
                Id = Guid.NewGuid(),
                Name = "Ahmad Manager",
                Email = "manager1@amman-re.com",
                PasswordHashed = BCrypt.Net.BCrypt.HashPassword("Manager@123"),
                RoleId = 2,
                TenantId = tenant1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        // الشركة الثانية: Zarqa Properties
        if (!await context.Tenants.AnyAsync(t => t.Id == tenant2Id))
        {
            await context.Tenants.AddAsync(new Tenant
            {
                Id = tenant2Id,
                Name = "Zarqa Properties",
                AddressCity = "Zarqa",
                AddressRegion = "New Zarqa",
                AddressStreet = "36th Street",
                Email = "contact@zarqa-prop.com",
                PhoneNumber = "0782222222",
                Status = Domain.Enums.TenantStatus.Active,
                CreatedAt = DateTime.UtcNow
            });

            // مدير الشركة الثانية
            await context.Users.AddAsync(new User
            {
                Id = Guid.NewGuid(),
                Name = "Sami Manager",
                Email = "manager2@zarqa-prop.com",
                PasswordHashed = BCrypt.Net.BCrypt.HashPassword("Manager@123"),
                RoleId = 2,
                TenantId = tenant2Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
    }
}