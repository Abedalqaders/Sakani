namespace Infrastructure
{
    using Microsoft.EntityFrameworkCore;
    using Domain.Entities;
    using Domain.Enums;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // تعريف الجداول (DbSets)
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Renter> Renters { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. تطبيق نظام التسمية snake_case لـ Postgres تلقائياً
            // يتم تفعيلها عادة في الـ Program.cs عبر .UseSnakeCaseNamingConvention()

            // 2. تطبيق الـ Global Query Filters (السحر الحقيقي)
            // هذا الفلتر يضمن أن أي استعلام لا يرجع البيانات المحذوفة منطقياً
            modelBuilder.Entity<Property>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Unit>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<Contract>().HasQueryFilter(c => !c.IsDeleted);

            // 3. إعدادات خاصة للـ Enums لتخزينها كـ SmallInt (TinyInt) [cite: 2026-01-31]
            modelBuilder.Entity<Contract>()
                .Property(c => c.ContractStatus)
                .HasConversion<byte>(); // يخزنها كـ byte في الكود و smallint في Postgres

            // 4. العلاقات (Relationships)
            modelBuilder.Entity<Property>()
                .HasMany(p => p.Units)
                .WithOne(u => u.Property)
                .HasForeignKey(u => u.PropertyId);
        }
    }
}
