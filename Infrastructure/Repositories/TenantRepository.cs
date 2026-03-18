using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure;

namespace Infrastructure.Repositories
{
    public class TenantRepository : GenericRepository<Tenant>, ITenantRepository
    {
        public TenantRepository(ApplicationDbContext context) : base(context)
        {

        }

     
    }
}