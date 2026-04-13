using Application.Common.Interfaces;
using Domain.Entities;


namespace Infrastructure.Repositories
{
 
    public class ImageRepository : GenericRepository<TicketImage>, IImageRepository
    {
        public ImageRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}