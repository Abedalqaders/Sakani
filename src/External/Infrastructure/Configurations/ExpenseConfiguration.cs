using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
          
            builder.ToTable("Expenses");



            builder.Property(e => e.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(e => e.ExpenseDate)
                .IsRequired();

            // 3. تحويل الـ Enums لقيم عددية (SmallInt/Byte)
            builder.Property(e => e.ExpenseType)
                .HasConversion<byte>();
           
   

            builder.HasOne(e => e.Property)
                .WithMany(p => p.Expenses)
                .HasForeignKey(e => e.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            // 5. الفلتر الخاص بالـ TenantId سيتم تعريفه في الـ DbContext كما اتفقنا سابقاً
        }
    }
}