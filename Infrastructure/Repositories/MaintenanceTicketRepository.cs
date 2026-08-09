using Application.Common.Interfaces.MaintenaceTicket;
using Application.Common.Interfaces.User;
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
    public class MaintenanceTicketRepository : GenericRepository<MaintenanceTicket>, IMaintenanceTicketRepository
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceTicketRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public override async Task<MaintenanceTicket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(t => t.Unit)
                .Include(t => t.Images)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public IQueryable<MaintenanceTicket> GetByRenterIdAsync(Guid renterId)
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


            return await query
                .OrderByDescending(t => t.CreatedAt)
                .AsNoTracking()
                .ToListAsync(ct);
        }
        public async Task<List<TicketResponseDto>> GetTicketsByRenterIdAsync(Guid renterId, CancellationToken ct)
        {
            return await _context.MaintenanceTickets
                .AsNoTracking()
                .Where(t => t.RenterId == renterId)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TicketResponseDto
                {
                    Id = t.Id,
                    UnitId = t.UnitId,
                    UnitNo = t.Unit.UnitNo,
                    Subject = t.Subject,
                    Description = t.Description,
                    Status = t.TicketStatus,
                    CreatedAt = t.CreatedAt,
                    Images = t.Images.Select(img => new TicketImageDto
                    {
                        Id = img.Id,
                        ImageUrl = img.ImagePath
                    }).ToList()
                })
                .ToListAsync(ct);
        }
    }
    }