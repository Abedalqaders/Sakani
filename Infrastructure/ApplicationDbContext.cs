namespace Infrastructure
{
    using Application.Common.Interfaces.User;
    using Application.Common.Interfaces.Tenant;
    using Domain.Common;
    using Domain.Entities;
    using Domain.Enums;
    using Microsoft.EntityFrameworkCore;
    using Application.Interfaces;

    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly Guid? _tenantId;
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ITenantService tenantService,
            ICurrentUserService currentUser)
            : base(options)
        {
            _currentUserService = currentUser;
            _tenantId = tenantService.GetTenantId();
        }

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
        public DbSet<TicketImage> Images { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            modelBuilder.Entity<Property>().HasQueryFilter(p => p.TenantId == _tenantId && !p.IsDeleted);
            modelBuilder.Entity<Unit>().HasQueryFilter(u => u.TenantId == _tenantId && !u.IsDeleted);
            modelBuilder.Entity<Contract>().HasQueryFilter(c => c.TenantId == _tenantId && !c.IsDeleted);
            modelBuilder.Entity<Renter>().HasQueryFilter(r => r.TenantId == _tenantId && !r.IsDeleted);
            modelBuilder.Entity<Expense>().HasQueryFilter(e => e.TenantId == _tenantId && !e.IsDeleted);
            modelBuilder.Entity<Payment>().HasQueryFilter(p => p.TenantId == _tenantId && !p.IsDeleted);
            modelBuilder.Entity<MaintenanceTicket>().HasQueryFilter(m => m.TenantId == _tenantId && !m.IsDeleted);
            modelBuilder.Entity<TicketImage>().HasQueryFilter(i => i.TenantId == _tenantId && !i.IsDeleted);
            modelBuilder.Entity<Notification>().HasQueryFilter(n => n.TenantId == _tenantId && !n.IsDeleted);
            modelBuilder.Entity<Tenant>().HasQueryFilter(t => !t.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
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

                        if (entry.Entity is TenantEntity tenantEntity)
                        {
                            if (tenantEntity.TenantId == Guid.Empty)
                            {
                                // رمي استثناء مباشر يمنع حفظ البيانات اليتيمة
                                tenantEntity.TenantId = _currentUserService.TenantId
                                    ?? throw new InvalidOperationException("Fatal error: Attempted to save tenant-specific data without a valid TenantId.");
                            }
                        }
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;
                        entry.Entity.UpdatedBy = userId;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.UpdatedAt = now;
                        entry.Entity.UpdatedBy = userId;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}