using Domain.Entities;
using Domain.Enums;
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
                Email = "manager@amman-re.com",
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
        var ammanProp1Id = Guid.Parse("3a59cc1c-f8d9-4ba4-9c9b-7ee0f5283af5");
        var ammanProp2Id = Guid.Parse("c2ce1c24-a891-4e58-af66-c3c5a1faff90");
        var zarqaPropId = Guid.Parse("4352708a-0656-404a-9475-da2622205340");

        // إضافة عقارات عمان
        if (!await context.Properties.IgnoreQueryFilters().AnyAsync(p => p.Id == ammanProp1Id))
        {
            await context.Properties.AddAsync(new Property
            {
                Id = ammanProp1Id, // ثبتنا الـ ID هون
                Name = "Abdali Gateway Tower",
                City = "Amman",
                AddressRegion = "Abdali",
                Street = "Al-Istishari St",
                BuildingNo = "10",
                PropertyType = PropertyType.Residential,
                TenantId = tenant1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Properties.IgnoreQueryFilters().AnyAsync(p => p.Id == ammanProp2Id))
        {
            await context.Properties.AddAsync(new Property
            {
                Id = ammanProp2Id, // ثبتنا الـ ID هون
                Name = "Jabal Amman Luxury Suites",
                City = "Amman",
                AddressRegion = "Jabal Amman",
                Street = "Rainbow Street",
                BuildingNo = "45",
                PropertyType = PropertyType.Residential,
                TenantId = tenant1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        // إضافة عقار الزرقاء
        if (!await context.Properties.IgnoreQueryFilters().AnyAsync(p => p.Id == zarqaPropId))
        {
            await context.Properties.AddAsync(new Property
            {
                Id = zarqaPropId, // ثبتنا الـ ID هون
                Name = "Zarqa Commercial Center",
                City = "Zarqa",
                AddressRegion = "New Zarqa",
                Street = "36th Street",
                BuildingNo = "102",
                PropertyType = PropertyType.Commercial,
                TenantId = tenant2Id,
                CreatedAt = DateTime.UtcNow
            });
        }
        // 4. تعريف المستأجرين (Renters)
        var renter1Id = Guid.Parse("7a123456-1111-2222-3333-444455556666");
        var renter2Id = Guid.Parse("8b123456-2222-3333-4444-555566667777");

        // مستأجر تابع لشركة عمان (Amman Real Estate)
        if (!await context.Renters.IgnoreQueryFilters().AnyAsync(r => r.Id == renter1Id))
        {
            var renterUser1Id = Guid.NewGuid();

            // إنشاء حساب مستخدم للمستأجر أولاً
            await context.Users.AddAsync(new User
            {
                Id = renterUser1Id,
                Name = "Omar Renter",
                Email = "omar@gmail.com",
                PasswordHashed = BCrypt.Net.BCrypt.HashPassword("Renter@123"),
                RoleId = 3, // Renter Role
                TenantId = tenant1Id, // يتبع لنفس شركة العقارات
                CreatedAt = DateTime.UtcNow
            });

            // إنشاء بيانات المستأجر
            await context.Renters.AddAsync(new Renter
            {
                Id = renter1Id,
                NationalId = "9901012345",
                PhoneNumber = "0790000001",
                Description = "Reliable tenant, works at Arab Bank",
                UserId = renterUser1Id,
                TenantId = tenant1Id, // عزل البيانات
                CreatedAt = DateTime.UtcNow
            });
        }

        // مستأجر تابع لشركة الزرقاء (Zarqa Properties)
        if (!await context.Renters.IgnoreQueryFilters().AnyAsync(r => r.Id == renter2Id))
        {
            var renterUser2Id = Guid.NewGuid();

            // حساب المستخدم
            await context.Users.AddAsync(new User
            {
                Id = renterUser2Id,
                Name = "Zaid Renter",
                Email = "zaid@gmail.com",
                PasswordHashed = BCrypt.Net.BCrypt.HashPassword("Renter@123"),
                RoleId = 3,
                TenantId = tenant2Id,
                CreatedAt = DateTime.UtcNow
            });

            // بيانات المستأجر
            await context.Renters.AddAsync(new Renter
            {
                Id = renter2Id,
                NationalId = "9952025566",
                PhoneNumber = "0780000002",
                Description = "Commercial tenant for Zarqa center",
                UserId = renterUser2Id,
                TenantId = tenant2Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
    }
}