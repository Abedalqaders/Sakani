namespace Infrastructure
{
    using Application.Common.Interfaces;
    using Domain.Common;
    using Domain.Entities;
    using Domain.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
    using Sakani.Application.Common.Interfaces;

    public class ApplicationDbContext : DbContext
    {
        private readonly Guid? _tenantId;
        private readonly ICurrentUserService _currentUserService;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService tenantService, ICurrentUserService currentUser)
         : base(options)
        {
            _currentUserService = currentUser;
            _tenantId = tenantService.GetTenantId();
        }

        // تعريف الجداول (DbSets)
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Renter> Renters { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<MaintenanceTicket> MaintenanceTickets { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // إضافة الأدوار الأساسي
            // 1. فلاتر البيانات (Tenant Isolation & Soft Delete)
            // جداول النظام العامة
            modelBuilder.Entity<Tenant>().HasQueryFilter(t => !t.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            // جداول الشركات (Isolation)



            modelBuilder.Entity<Property>().HasQueryFilter(p => p.TenantId == _tenantId && !p.IsDeleted);
            modelBuilder.Entity<Unit>().HasQueryFilter(u => u.TenantId == _tenantId && !u.IsDeleted);
            modelBuilder.Entity<Contract>().HasQueryFilter(c => c.TenantId == _tenantId && !c.IsDeleted);
            modelBuilder.Entity<Renter>().HasQueryFilter(r => r.TenantId == _tenantId && !r.IsDeleted);
            modelBuilder.Entity<Expense>().HasQueryFilter(e => e.TenantId == _tenantId && !e.IsDeleted);
            modelBuilder.Entity<Payment>().HasQueryFilter(p => p.TenantId == _tenantId && !p.IsDeleted);
            modelBuilder.Entity<MaintenanceTicket>().HasQueryFilter(m => m.TenantId == _tenantId && !m.IsDeleted);

            // 2. تحويل الـ Enums لقيم عددية (SmallInt)
            modelBuilder.Entity<Contract>().Property(c => c.ContractStatus).HasConversion<byte>();
            modelBuilder.Entity<Tenant>().Property(t => t.Status).HasConversion<byte>();
            modelBuilder.Entity<Expense>().Property(e => e.ExpenseType).HasConversion<byte>();

            // 3. بناء العلاقات كما وردت في الـ ERD

            // العقارات والوحدات (Property -> Units)
            modelBuilder.Entity<Property>()
                .HasMany(p => p.Units)
                .WithOne(u => u.Property)
                .HasForeignKey(u => u.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            // العقارات والمصاريف (Property -> Expenses)
            modelBuilder.Entity<Property>()
                .HasMany(p => p.Expenses)
                .WithOne(e => e.Property)
                .HasForeignKey(e => e.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // الوحدات والعقود (Unit -> Contracts)
            modelBuilder.Entity<Unit>()
                .HasMany(u => u.Contracts)
                .WithOne(c => c.Unit)
                .HasForeignKey(c => c.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            // المستأجرين والعقود (Renter -> Contracts)
            modelBuilder.Entity<Renter>()
                .HasMany(r => r.Contracts)
                .WithOne(c => c.Renter)
                .HasForeignKey(c => c.RenterId)
                .OnDelete(DeleteBehavior.Restrict);

            // العقود والدفعات (Contract -> Payments)
            modelBuilder.Entity<Contract>()
                .HasMany(c => c.Payments)
                .WithOne(p => p.Contract)
                .HasForeignKey(p => p.ContractId)
                .OnDelete(DeleteBehavior.Restrict);

            // المستخدمين والمستأجرين (User -> Renter)
            // يسمح بوجود مستأجر بدون حساب (UserId = Null)
            modelBuilder.Entity<Renter>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // الشركات والمستخدمين (Tenant -> Users)
            modelBuilder.Entity<Tenant>()
                .HasMany<User>()
                .WithOne()
                .HasForeignKey(u => u.TenantId)
                .IsRequired(false) // يسمح بمدير نظام (Super Admin)
                .OnDelete(DeleteBehavior.Restrict);

            // الأدوار والمستخدمين (Role -> Users)
            modelBuilder.Entity<Role>()
                .HasMany<User>()
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                var now = DateTime.UtcNow;
                var userId = _currentUserService.UserId;

                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.CreatedBy = userId;

                        // هنا السحر: إذا كان الكيان يتبع لشركة (يرث من TenantEntity)
                        if (entry.Entity is TenantEntity tenantEntity)
                        {
                            // نحقن الـ TenantId تلقائياً من التوكن إذا كان فارغاً
                            if (tenantEntity.TenantId == Guid.Empty)
                            {
                                tenantEntity.TenantId = _currentUserService.TenantId ?? Guid.Empty;
                            }
                        }
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;
                        entry.Entity.UpdatedBy = userId;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; // نمنع الحذف الفعلي ونحوله لتعديل
                        entry.Entity.IsDeleted = true;      // نغير حالة الحقل
                        entry.Entity.UpdatedAt = now;       // نوثق متى تم الحذف
                        entry.Entity.UpdatedBy = userId;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
    }
