using Application.Common.Interfaces.MaintenaceTicket;
using Application.Dto.MaintenanceTicket;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    // 1. الوراثة من GenericRepository توفر لك Add, Update, Delete, GetByIdAsync مجاناً
    public class MaintenanceTicketRepository : GenericRepository<MaintenanceTicket>, IMaintenanceTicketRepository
    {
        private readonly ApplicationDbContext _context;

        // 2. تمرير الـ DbContext للـ Base Class
        public MaintenanceTicketRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public override async Task<MaintenanceTicket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(t => t.Unit)    // لجلب بيانات الشقة
                .Include(t => t.Images)  // لجلب الصور
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }
        // 3. كتابة الدوال المخصصة فقط التي لا توجد في الـ Generic Repository
        public  IQueryable<MaintenanceTicket> GetByRenterIdAsync(Guid renterId)
        {
            return _context.MaintenanceTickets.AsNoTracking();
        }
        public async Task<IReadOnlyList<MaintenanceTicket>> GetFilteredAsync(TicketFilterDto filter, CancellationToken ct = default)
        {
            var query = _context.MaintenanceTickets.AsQueryable();

            if (filter.Status.HasValue)
                query = query.Where(t => t.TicketStatus == filter.Status.Value);

            if (filter.UnitId.HasValue)
                query = query.Where(t => t.UnitId == filter.UnitId.Value);

            if (filter.RenterId.HasValue)
                query = query.Where(t => t.RenterId == filter.RenterId.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                query = query.Where(t =>
                    t.Subject.Contains(filter.SearchTerm) ||
                    t.Description.Contains(filter.SearchTerm));

            if (filter.FromDate.HasValue)
                query = query.Where(t => t.CreatedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(t => t.CreatedAt <= filter.ToDate.Value);

            // ننفذ الاستعلام في النهاية
            return await query
                .OrderByDescending(t => t.CreatedAt)
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}