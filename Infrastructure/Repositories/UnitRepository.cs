using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UnitRepository :GenericRepository<Unit>, IUnitRepository
    {


        public UnitRepository(ApplicationDbContext context) : base(context)
        {
        
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
    }
}
