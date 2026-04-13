using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;
public class ImageConfiguration : IEntityTypeConfiguration<TicketImage>
{
    public void Configure(EntityTypeBuilder<TicketImage> builder)
    {
        builder.Property(i => i.ImagePath).IsRequired().HasMaxLength(500);

        // العلاقة: الصورة تتبع تذكرة واحدة، والتذكرة لها صور كثيرة
        builder.HasOne(i => i.Ticket)
            .WithMany(t => t.Images)
            .HasForeignKey(i => i.TicketId)
            .OnDelete(DeleteBehavior.Cascade); // هنا نستخدم Cascade: إذا حُذفت التذكرة، تُحذف سجلات صورها تلقائياً
    }
}