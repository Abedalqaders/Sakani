using Application.Common.Interfaces.Property;
using Domain.Entities;
namespace Infrastructure.Repositories
{
    public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
    {
        public PropertyRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
