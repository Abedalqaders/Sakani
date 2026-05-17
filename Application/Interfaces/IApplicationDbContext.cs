using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure; // مطلوب هنا
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Payment> Payments { get; }
    DbSet<Contract> Contracts { get; }
    DbSet<MaintenanceTicket> MaintenanceTickets { get; }
    DbSet<Unit> Units { get; }
    DbSet<Notification> Notifications { get; }


    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}