using Domain.Entities;
using Domain.Enums;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // 0. تثبيت معرفات المستخدمين لربطها بالجداول الأخرى والإشعارات
        var superAdminUserId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var tenant1ManagerUserId = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var tenant2ManagerUserId = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var renterUser1Id = Guid.Parse("30000000-0000-0000-0000-000000000001");
        var renterUser2Id = Guid.Parse("30000000-0000-0000-0000-000000000002");

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

        // 2. إنشاء الـ SuperAdmin
        if (!await context.Users.AnyAsync(u => u.Id == superAdminUserId))
        {
            var superAdmin = new User
            {
                Id = superAdminUserId,
                Name = "The Boss",
                Email = "super@sakani.com",
                PasswordHashed = BCrypt.Net.BCrypt.HashPassword("Super@123"),
                RoleId = 1,
                TenantId = null,
                CreatedAt = DateTime.UtcNow
            };
            await context.Users.AddAsync(superAdmin);
        }

        // 3. تعريف الشركات (Tenants)
        var tenant1Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35");
        var tenant2Id = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479");

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
                Status = TenantStatus.Active,
                CreatedAt = DateTime.UtcNow
            });

            await context.Users.AddAsync(new User
            {
                Id = tenant1ManagerUserId,
                Name = "Ahmad Manager",
                Email = "manager@amman-re.com",
                PasswordHashed = BCrypt.Net.BCrypt.HashPassword("Manager@123"),
                RoleId = 2,
                TenantId = tenant1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

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
                Status = TenantStatus.Active,
                CreatedAt = DateTime.UtcNow
            });

            await context.Users.AddAsync(new User
            {
                Id = tenant2ManagerUserId,
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

        // 4. إضافة العقارات
        if (!await context.Properties.IgnoreQueryFilters().AnyAsync(p => p.Id == ammanProp1Id))
        {
            await context.Properties.AddAsync(new Property
            {
                Id = ammanProp1Id,
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
                Id = ammanProp2Id,
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

        if (!await context.Properties.IgnoreQueryFilters().AnyAsync(p => p.Id == zarqaPropId))
        {
            await context.Properties.AddAsync(new Property
            {
                Id = zarqaPropId,
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

        // 5. تعريف المستأجرين (Renters)
        var renter1Id = Guid.Parse("7a123456-1111-2222-3333-444455556666");
        var renter2Id = Guid.Parse("8b123456-2222-3333-4444-555566667777");

        if (!await context.Renters.IgnoreQueryFilters().AnyAsync(r => r.Id == renter1Id))
        {
            await context.Users.AddAsync(new User
            {
                Id = renterUser1Id,
                Name = "Omar Renter",
                Email = "omar@gmail.com",
                PasswordHashed = BCrypt.Net.BCrypt.HashPassword("Renter@123"),
                RoleId = 3,
                TenantId = tenant1Id,
                CreatedAt = DateTime.UtcNow
            });

            await context.Renters.AddAsync(new Renter
            {
                Id = renter1Id,
                FirstName = "Mohhmad",
                LastName = "Alaio",
                NationalId = "9901012345",
                PhoneNumber = "0790000001",
                Description = "Reliable tenant, works at Arab Bank",
                UserId = renterUser1Id,
                TenantId = tenant1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Renters.IgnoreQueryFilters().AnyAsync(r => r.Id == renter2Id))
        {
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

            await context.Renters.AddAsync(new Renter
            {
                Id = renter2Id,
                FirstName = "AbedAlqader",
                LastName = "Alsadi",
                NationalId = "9952025566",
                PhoneNumber = "0780000002",
                Description = "Commercial tenant for Zarqa center",
                UserId = renterUser2Id,
                TenantId = tenant2Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        // 6. تعريف الوحدات (Units)
        var unit1Id = Guid.Parse("11111111-2222-3333-4444-555566667777");
        var unit2Id = Guid.Parse("22222222-3333-4444-5555-666677778888");
        var unit3Id = Guid.Parse("33333333-4444-5555-6666-777788889999");

        if (!await context.Units.IgnoreQueryFilters().AnyAsync(u => u.Id == unit1Id))
        {
            await context.Units.AddAsync(new Unit
            {
                Id = unit1Id,
                UnitNo = "A-101",
                Floor = "First",
                RentPrice = 500,
                Area = "120",
                PropertyId = ammanProp1Id,
                UnitStatus = UnitStatus.Rented,
                TenantId = tenant1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Units.IgnoreQueryFilters().AnyAsync(u => u.Id == unit2Id))
        {
            await context.Units.AddAsync(new Unit
            {
                Id = unit2Id,
                UnitNo = "C-50",
                Floor = "First",
                RentPrice = 1200,
                Area = "250",
                PropertyId = zarqaPropId,
                UnitStatus = UnitStatus.Rented,
                TenantId = tenant2Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Units.IgnoreQueryFilters().AnyAsync(u => u.Id == unit3Id))
        {
            await context.Units.AddAsync(new Unit
            {
                Id = unit3Id,
                UnitNo = "B-202",
                Floor = "Second",
                RentPrice = 450,
                Area = "110",
                PropertyId = ammanProp1Id,
                UnitStatus = UnitStatus.Available,
                TenantId = tenant1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        // 7. العقود والدفعات (Contracts & Payments)
        var contract1Id = Guid.Parse("99999999-8888-7777-6666-555544443333");
        if (!await context.Contracts.IgnoreQueryFilters().AnyAsync(c => c.Id == contract1Id))
        {
            var contract1 = new Contract
            {
                Id = contract1Id,
                StartDate = DateTime.UtcNow.AddMonths(-1),
                EndDate = DateTime.UtcNow.AddMonths(11),
                RentAmount = 6000,
                PaymentFreq = PaymentFrequency.Monthly,
                ContractStatus = ContractStatus.Active,
                UnitId = unit1Id,
                RenterId = renter1Id,
                TenantId = tenant1Id,
                CreatedAt = DateTime.UtcNow,
                Payments = new List<Payment>()
            };

            for (int i = 0; i < 12; i++)
            {
                contract1.Payments.Add(new Payment
                {
                    Id = Guid.NewGuid(),
                    Amount = 500,
                    DueDate = contract1.StartDate.AddMonths(i),
                    PaymentStatus = i == 0 ? PaymentStatus.Paid : PaymentStatus.Pending,
                    PaymentDate = i == 0 ? DateTime.UtcNow.AddMonths(-1) : null,
                    TenantId = tenant1Id,
                    CreatedAt = DateTime.UtcNow
                });
            }
            await context.Contracts.AddAsync(contract1);
        }

        var contract2Id = Guid.Parse("77777777-6666-5555-4444-333322221111");
        if (!await context.Contracts.IgnoreQueryFilters().AnyAsync(c => c.Id == contract2Id))
        {
            var contract2 = new Contract
            {
                Id = contract2Id,
                StartDate = DateTime.UtcNow.AddDays(-40),
                EndDate = DateTime.UtcNow.AddMonths(5),
                RentAmount = 7200,
                PaymentFreq = PaymentFrequency.Monthly,
                ContractStatus = ContractStatus.Active,
                UnitId = unit2Id,
                RenterId = renter2Id,
                TenantId = tenant2Id,
                CreatedAt = DateTime.UtcNow,
                Payments = new List<Payment>()
            };

            contract2.Payments.Add(new Payment
            {
                Id = Guid.NewGuid(),
                Amount = 1200,
                DueDate = contract2.StartDate,
                PaymentStatus = PaymentStatus.Paid,
                PaymentDate = contract2.StartDate.AddDays(1),
                TenantId = tenant2Id,
                CreatedAt = DateTime.UtcNow
            });

            contract2.Payments.Add(new Payment
            {
                Id = Guid.NewGuid(),
                Amount = 1200,
                DueDate = DateTime.UtcNow.AddDays(-10),
                PaymentStatus = PaymentStatus.Pending,
                TenantId = tenant2Id,
                CreatedAt = DateTime.UtcNow
            });

            contract2.Payments.Add(new Payment
            {
                Id = Guid.NewGuid(),
                Amount = 1200,
                DueDate = DateTime.UtcNow.AddDays(20),
                PaymentStatus = PaymentStatus.Pending,
                TenantId = tenant2Id,
                CreatedAt = DateTime.UtcNow
            });

            await context.Contracts.AddAsync(contract2);
        }

        await context.SaveChangesAsync();

        // 8. تحديث الدفعات المتأخرة برمجياً
        var expiredPendingPayments = await context.Set<Payment>()
            .IgnoreQueryFilters()
            .Where(p => p.PaymentStatus == PaymentStatus.Pending && p.DueDate < DateTime.UtcNow)
            .ToListAsync();

        if (expiredPendingPayments.Count > 0)
        {
            foreach (var payment in expiredPendingPayments)
            {
                payment.PaymentStatus = PaymentStatus.Overdue;
            }
            await context.SaveChangesAsync();
        }

        // 9. تعريف المصاريف (Expenses)
        var expense1Id = Guid.Parse("aaaa1111-2222-3333-4444-555566667777");
        var expense2Id = Guid.Parse("bbbb1111-2222-3333-4444-555566667777");
        var expense3Id = Guid.Parse("cccc1111-2222-3333-4444-555566667777");

        if (!await context.Expenses.IgnoreQueryFilters().AnyAsync(e => e.Id == expense1Id))
        {
            await context.Expenses.AddAsync(new Expense
            {
                Id = expense1Id,
                PropertyId = ammanProp1Id,
                UnitId = unit1Id,
                Amount = 150,
                ExpenseType = ExpenseType.Maintenance,
                ExpenseDate = DateTime.UtcNow.AddDays(-10),
                Description = "AC repair for unit A-101",
                TenantId = tenant1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Expenses.IgnoreQueryFilters().AnyAsync(e => e.Id == expense2Id))
        {
            await context.Expenses.AddAsync(new Expense
            {
                Id = expense2Id,
                PropertyId = zarqaPropId,
                UnitId = unit2Id,
                Amount = 300,
                ExpenseType = ExpenseType.Utility,
                ExpenseDate = DateTime.UtcNow.AddDays(-5),
                Description = "Electricity bill for commercial unit",
                TenantId = tenant2Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Expenses.IgnoreQueryFilters().AnyAsync(e => e.Id == expense3Id))
        {
            await context.Expenses.AddAsync(new Expense
            {
                Id = expense3Id,
                PropertyId = ammanProp2Id,
                UnitId = null,
                Amount = 500,
                ExpenseType = ExpenseType.Other,
                ExpenseDate = DateTime.UtcNow.AddDays(-2),
                Description = "Building cleaning service",
                TenantId = tenant1Id,
                CreatedAt = DateTime.UtcNow
            });
        }
        await context.SaveChangesAsync();

        // 10. الإشعارات (Notifications)
        if (!await context.Set<Notification>().IgnoreQueryFilters().AnyAsync())
        {
            var notifications = new List<Notification>
            {
                // إشعار لصاحب العقار (Ahmad Manager) يخبره بوجود دفعة متأخرة
                new Notification
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenant2Id,
                    UserId = tenant2ManagerUserId,
                    SenderId = null, // النظام هو المرسل
                    Title = "دفعة إيجار متأخرة",
                    Message = "يوجد دفعة متأخرة على العقد رقم 2",
                    Type = NotificationType.PaymentOverdue,
                    ReferenceId = contract2Id, // التوجيه لتفاصيل العقد
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                },
                
                // إشعار للمستأجر (Omar Renter) يذكره بموعد الدفع القادم
                new Notification
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenant1Id,
                    UserId = renterUser1Id,
                    SenderId = tenant1ManagerUserId, // المالك هو المرسل
                    Title = "تذكير بموعد الدفع",
                    Message = "يرجى العلم أن موعد الدفعة القادمة سيحل قريباً.",
                    Type = NotificationType.PaymentReminder,
                    ReferenceId = contract1Id,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                },

                // إشعار للمستأجر (Zaid Renter) بتحديث حالة الصيانة
                new Notification
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenant2Id,
                    UserId = renterUser2Id,
                    SenderId = tenant2ManagerUserId,
                    Title = "تحديث طلب الصيانة",
                    Message = "تم الانتهاء من صيانة المكيف في وحدتك التجارية.",
                    Type = NotificationType.MaintenanceUpdate,
                    ReferenceId = expense2Id,
                    IsRead = true,
                    ReadAt = DateTime.UtcNow.AddMinutes(-30),
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                }
            };

            await context.Set<Notification>().AddRangeAsync(notifications);
            await context.SaveChangesAsync();
        }
    }
}