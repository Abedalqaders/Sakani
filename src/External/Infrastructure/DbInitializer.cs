using Domain.Entities;
using Domain.Enums;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class DbInitializer
{
    // الدالة الأولى: البيانات الأساسية التي يحتاجها النظام للعمل في بيئة الإنتاج
    public static async Task SeedSystemEssentialsAsync(ApplicationDbContext context)
    {
        var superAdminUserId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var now = DateTime.UtcNow;

        if (!await context.Roles.AnyAsync())
        {
            await context.Roles.AddRangeAsync(new List<Role>
            {
                new Role { Id = 1, Name = "SuperAdmin" },
                new Role { Id = 2, Name = "Tenant" },
                new Role { Id = 3, Name = "Renter" }
            });
            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync(u => u.Id == superAdminUserId))
        {
            await context.Users.AddAsync(new User
            {
                Id = superAdminUserId,
                Name = "The Boss",
                Email = "super@sakani.com",
                PasswordHashed = BCrypt.Net.BCrypt.HashPassword("Super@123"),
                RoleId = 1,
                TenantId = null,
                CreatedAt = now
            });
            await context.SaveChangesAsync();
        }
    }

    // الدالة الثانية: البيانات الوهمية المخصصة لبيئة التطوير والاختبار
    public static async Task SeedDummyDataAsync(ApplicationDbContext context)
    {
        var tenant1ManagerUserId = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var tenant2ManagerUserId = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var renterUser1Id = Guid.Parse("30000000-0000-0000-0000-000000000001");
        var renterUser2Id = Guid.Parse("30000000-0000-0000-0000-000000000002");
        var tenant1Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35");
        var tenant2Id = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479");
        var now = DateTime.UtcNow;

        if (!await context.Tenants.AnyAsync(t => t.Id == tenant1Id))
        {
            await context.Tenants.AddAsync(new Tenant { Id = tenant1Id, Name = "Amman Real Estate Co", AddressCity = "Amman", AddressRegion = "Abdali", AddressStreet = "Queen Rania St", Email = "info@amman-re.com", PhoneNumber = "0791111111", Status = TenantStatus.Active, CreatedAt = now });
            await context.Users.AddAsync(new User { Id = tenant1ManagerUserId, Name = "Ahmad Manager", Email = "manager@amman-re.com", PasswordHashed = BCrypt.Net.BCrypt.HashPassword("Manager@123"), RoleId = 2, TenantId = tenant1Id, CreatedAt = now });
        }

        if (!await context.Tenants.AnyAsync(t => t.Id == tenant2Id))
        {
            await context.Tenants.AddAsync(new Tenant { Id = tenant2Id, Name = "Zarqa Properties", AddressCity = "Zarqa", AddressRegion = "New Zarqa", AddressStreet = "36th Street", Email = "contact@zarqa-prop.com", PhoneNumber = "0782222222", Status = TenantStatus.Active, CreatedAt = now });
            await context.Users.AddAsync(new User { Id = tenant2ManagerUserId, Name = "Sami Manager", Email = "manager2@zarqa-prop.com", PasswordHashed = BCrypt.Net.BCrypt.HashPassword("Manager@123"), RoleId = 2, TenantId = tenant2Id, CreatedAt = now });
        }
        await context.SaveChangesAsync();

        var ammanProp1Id = Guid.Parse("3a59cc1c-f8d9-4ba4-9c9b-7ee0f5283af5");
        var ammanProp2Id = Guid.Parse("c2ce1c24-a891-4e58-af66-c3c5a1faff90");
        var zarqaPropId = Guid.Parse("4352708a-0656-404a-9475-da2622205340");
        var renter1Id = Guid.Parse("7a123456-1111-2222-3333-444455556666");
        var renter2Id = Guid.Parse("8b123456-2222-3333-4444-555566667777");

        if (!await context.Properties.IgnoreQueryFilters().AnyAsync(p => p.Id == ammanProp1Id))
        {
            await context.Properties.AddAsync(new Property { Id = ammanProp1Id, Name = "Abdali Gateway Tower", City = "Amman", AddressRegion = "Abdali", Street = "Arar St", BuildingNo = "15", PropertyType = PropertyType.Residential, TenantId = tenant1Id, CreatedAt = now });
            await context.Properties.AddAsync(new Property { Id = ammanProp2Id, Name = "Jabal Amman Luxury Suites", City = "Amman", AddressRegion = "Jabal Amman", Street = "Rainbow St", BuildingNo = "22", PropertyType = PropertyType.Residential, TenantId = tenant1Id, CreatedAt = now });
            await context.Properties.AddAsync(new Property { Id = zarqaPropId, Name = "Zarqa Commercial Center", City = "Zarqa", AddressRegion = "New Zarqa", Street = "36th St", BuildingNo = "104", PropertyType = PropertyType.Commercial, TenantId = tenant2Id, CreatedAt = now });

            await context.Users.AddAsync(new User { Id = renterUser1Id, Name = "Omar Renter", Email = "omar@gmail.com", PasswordHashed = BCrypt.Net.BCrypt.HashPassword("Renter@123"), RoleId = 3, TenantId = tenant1Id, CreatedAt = now });
            await context.Renters.AddAsync(new Renter { Id = renter1Id, FirstName = "Omar", LastName = "Alaio", NationalId = "9901012345", PhoneNumber = "0790000001", UserId = renterUser1Id, TenantId = tenant1Id, CreatedAt = now, Description = "Regular Renter" });

            await context.Users.AddAsync(new User { Id = renterUser2Id, Name = "Zaid Renter", Email = "zaid@gmail.com", PasswordHashed = BCrypt.Net.BCrypt.HashPassword("Renter@123"), RoleId = 3, TenantId = tenant2Id, CreatedAt = now });
            await context.Renters.AddAsync(new Renter { Id = renter2Id, FirstName = "AbedAlqader", LastName = "Alsadi", NationalId = "9952025566", PhoneNumber = "0780000002", UserId = renterUser2Id, TenantId = tenant2Id, CreatedAt = now, Description = "Commercial Renter" });

            await context.SaveChangesAsync();
        }

        var unit1Id = Guid.Parse("11111111-2222-3333-4444-555566667777");
        var unit2Id = Guid.Parse("22222222-3333-4444-5555-666677778888");
        var unit3Id = Guid.Parse("33333333-4444-5555-6666-777788889999");

        if (!await context.Units.IgnoreQueryFilters().AnyAsync(u => u.Id == unit1Id))
        {
            await context.Units.AddAsync(new Unit { Id = unit1Id, UnitNo = "A-101", Floor = "1", Area = "120sqm", RentPrice = 500, PropertyId = ammanProp1Id, UnitStatus = UnitStatus.Rented, IsVacancyNotified = false, TenantId = tenant1Id, CreatedAt = now });
            await context.Units.AddAsync(new Unit { Id = unit2Id, UnitNo = "C-50", Floor = "5", Area = "250sqm", RentPrice = 1200, PropertyId = zarqaPropId, UnitStatus = UnitStatus.Rented, IsVacancyNotified = false, TenantId = tenant2Id, CreatedAt = now });
            await context.Units.AddAsync(new Unit { Id = unit3Id, UnitNo = "B-202", Floor = "2", Area = "110sqm", RentPrice = 450, PropertyId = ammanProp1Id, UnitStatus = UnitStatus.Rented, IsVacancyNotified = false, TenantId = tenant1Id, CreatedAt = now });
            await context.SaveChangesAsync();
        }

        var contract1Id = Guid.Parse("99999999-8888-7777-6666-555544443333");
        if (!await context.Contracts.IgnoreQueryFilters().AnyAsync(c => c.Id == contract1Id))
        {
            var contract1 = new Contract
            {
                Id = contract1Id,
                StartDate = now.AddMonths(-1),
                EndDate = now.AddMonths(11),
                RentAmount = 6000,
                PaymentFreq = PaymentFrequency.Monthly,
                ContractStatus = ContractStatus.Active,
                IsExpirationReminderSent = false,
                IsOverstayNotificationSent = false,
                UnitId = unit1Id,
                RenterId = renter1Id,
                TenantId = tenant1Id,
                CreatedAt = now,
                Payments = new List<Payment>()
            };

            for (int i = 0; i < 12; i++)
            {
                var dueDate = i == 1 ? now.AddDays(-3) : contract1.StartDate.AddMonths(i);
                var paymentStatus = i == 0 ? PaymentStatus.Paid : PaymentStatus.Pending;
                var paymentDate = i == 0 ? now.AddMonths(-1) : (DateTime?)null;

                contract1.Payments.Add(new Payment
                {
                    Id = Guid.NewGuid(),
                    Amount = 500,
                    DueDate = dueDate,
                    PaymentStatus = paymentStatus,
                    PaymentDate = paymentDate,
                    IsOverdueNotificationSent = false,
                    TenantId = tenant1Id,
                    CreatedAt = now
                });
            }
            await context.Contracts.AddAsync(contract1);
        }

        var contract2Id = Guid.Parse("77777777-6666-5555-4444-333322221111");
        if (!await context.Contracts.IgnoreQueryFilters().AnyAsync(c => c.Id == contract2Id))
        {
            await context.Contracts.AddAsync(new Contract
            {
                Id = contract2Id,
                StartDate = now.AddMonths(-11),
                EndDate = now.AddDays(15),
                RentAmount = 7200,
                PaymentFreq = PaymentFrequency.Monthly,
                ContractStatus = ContractStatus.Active,
                IsExpirationReminderSent = false,
                IsOverstayNotificationSent = false,
                UnitId = unit2Id,
                RenterId = renter2Id,
                TenantId = tenant2Id,
                CreatedAt = now
            });
        }

        var contract3Id = Guid.Parse("66666666-5555-4444-3333-222211110000");
        if (!await context.Contracts.IgnoreQueryFilters().AnyAsync(c => c.Id == contract3Id))
        {
            await context.Contracts.AddAsync(new Contract
            {
                Id = contract3Id,
                StartDate = now.AddMonths(-12),
                EndDate = now.AddDays(-5),
                RentAmount = 5400,
                PaymentFreq = PaymentFrequency.Monthly,
                ContractStatus = ContractStatus.Active,
                IsExpirationReminderSent = false,
                IsOverstayNotificationSent = false,
                UnitId = unit3Id,
                RenterId = renter1Id,
                TenantId = tenant1Id,
                CreatedAt = now
            });
        }

        var ticket1Id = Guid.Parse("55555555-4444-3333-2222-111100009999");
        if (!await context.MaintenanceTickets.IgnoreQueryFilters().AnyAsync(t => t.Id == ticket1Id))
        {
            await context.MaintenanceTickets.AddAsync(new MaintenanceTicket
            {
                Id = ticket1Id,
                UnitId = unit1Id,
                RenterId = renter1Id,
                Subject = "Water Leakage",
                TicketStatus = TicketStatus.Open,
                Description = "Water leak escalation test",
                IsEscalationNotified = false,
                TenantId = tenant1Id,
                CreatedAt = now.AddDays(-3)
            });
        }
        await context.SaveChangesAsync();

        var expense1Id = Guid.Parse("aaaa1111-2222-3333-4444-555566667777");
        var expense2Id = Guid.Parse("bbbb1111-2222-3333-4444-555566667777");
        var expense3Id = Guid.Parse("cccc1111-2222-3333-4444-555566667777");

        if (!await context.Expenses.IgnoreQueryFilters().AnyAsync(e => e.Id == expense1Id))
        {
            await context.Expenses.AddAsync(new Expense { Id = expense1Id, PropertyId = ammanProp1Id, UnitId = unit1Id, Amount = 150, ExpenseType = ExpenseType.Maintenance, ExpenseDate = now.AddDays(-10), Description = "AC repair for unit A-101", TenantId = tenant1Id, CreatedAt = now });
        }

        if (!await context.Expenses.IgnoreQueryFilters().AnyAsync(e => e.Id == expense2Id))
        {
            await context.Expenses.AddAsync(new Expense { Id = expense2Id, PropertyId = zarqaPropId, UnitId = unit2Id, Amount = 300, ExpenseType = ExpenseType.Utility, ExpenseDate = now.AddDays(-5), Description = "Electricity bill for commercial unit", TenantId = tenant2Id, CreatedAt = now });
        }

        if (!await context.Expenses.IgnoreQueryFilters().AnyAsync(e => e.Id == expense3Id))
        {
            await context.Expenses.AddAsync(new Expense { Id = expense3Id, PropertyId = ammanProp2Id, UnitId = null, Amount = 500, ExpenseType = ExpenseType.Other, ExpenseDate = now.AddDays(-2), Description = "Building cleaning service", TenantId = tenant1Id, CreatedAt = now });
        }
        await context.SaveChangesAsync();

        if (!await context.Notifications.IgnoreQueryFilters().AnyAsync())
        {
            await context.Notifications.AddRangeAsync(new List<Notification>
            {
                new Notification { Id = Guid.NewGuid(), UserId = tenant1ManagerUserId, SenderId = null, Title = "تأخر في سداد الدفعة", Message = "تنبيه: المستأجر عمر العليان متأخر في سداد الدفعة المستحقة للعقد رقم 1.", Type = NotificationType.PaymentOverdue, ReferenceId = contract1Id, IsRead = false, TenantId = tenant1Id, CreatedAt = now },
                new Notification { Id = Guid.NewGuid(), UserId = tenant1ManagerUserId, SenderId = null, Title = "تصعيد تذكرة صيانة متأخرة", Message = "تم تصعيد تذكرة تسريب المياه لعدم اتخاذ إجراء ضمن المدة المحددة.", Type = NotificationType.MaintenanceEscalation, ReferenceId = ticket1Id, IsRead = false, TenantId = tenant1Id, CreatedAt = now.AddHours(-2) },
                new Notification { Id = Guid.NewGuid(), UserId = tenant2ManagerUserId, SenderId = null, Title = "تنبيه: اقتراب انتهاء صلاحية العقد", Message = "العقد رقم 2 الخاص بالوحدة C-50 سينتهي خلال 15 يوماً.", Type = NotificationType.ContractRenewalReminder, ReferenceId = contract2Id, IsRead = true, ReadAt = now.AddHours(-1), TenantId = tenant2Id, CreatedAt = now.AddDays(-1) }
            });
            await context.SaveChangesAsync();
        }
    }
}