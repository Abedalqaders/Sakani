using Application.Common.Interfaces.Renter;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class RenterRepository : GenericRepository<Renter>, IRenterRepository
    {
        public RenterRepository(ApplicationDbContext Context) : base(Context)
        {
        }
        public async Task<Renter?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.Renters.IgnoreQueryFilters()
                .FirstOrDefaultAsync(r => r.UserId == userId, ct);
        }
    }
}

    

