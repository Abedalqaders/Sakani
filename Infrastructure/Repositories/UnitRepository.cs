using Application.Common.Interfaces.Unit;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UnitRepository :GenericRepository<Unit>, IUnitRepository
    {


        public UnitRepository(ApplicationDbContext context) : base(context)
        {
        
        }
        public async Task<Guid> GetUnitIdByRenter(Guid renterid,CancellationToken ct)
        {
            return await _context.Contracts.AsNoTracking()
         .Where(u => u.RenterId == renterid)
         .Select(u => u.UnitId)
         .FirstOrDefaultAsync(ct);
        }
        public async Task<IReadOnlyList<Unit>> GetAllWithPropertyAsync(CancellationToken ct)
        {
            return await _context.Units
                .Include(u => u.Property)
                .AsNoTracking()
                .ToListAsync(ct);
        }
        public async Task<IReadOnlyList<Unit>> GetUnitsByPropertyAsync(Guid propertyId, CancellationToken ct)
        {
            return await _context.Units
                .Where(u => u.PropertyId == propertyId) // الفلترة حسب العمارة
                .Include(u => u.Property)               // عشان نجيب اسم العمارة برضه
                .AsNoTracking()
                .ToListAsync(ct);
        }
        public async Task<Unit?> GetByIdWithPropertyAsync(Guid id, CancellationToken ct)
        {
            return await _context.Units
                .Include(u => u.Property)
                .FirstOrDefaultAsync(u => u.Id == id, ct);
        }
        public async Task<decimal> GetOccupancyRateAsync(CancellationToken ct)
        {
       
            var stats = await _context.Units
                .AsNoTracking()
                .GroupBy(_ => 1) 
                .Select(g => new
                {
                    Total = g.Count(),
                    Rented = g.Count(u => u.UnitStatus == UnitStatus.Rented)
                })
                .FirstOrDefaultAsync(ct);

            if (stats == null || stats.Total == 0) return 0m;

            return Math.Round((decimal)stats.Rented / stats.Total * 100, 2);
        }

    }
}
