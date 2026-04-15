using Application.Dto.MaintenanceTicket;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IMaintenanceTicketRepository: IGenericRepository<MaintenanceTicket>
    {
        
        Task<IReadOnlyList<MaintenanceTicket>> GetByRenterIdAsync(Guid renterId, CancellationToken ct = default);
        Task<IReadOnlyList<MaintenanceTicket>> GetFilteredAsync(TicketFilterDto filter, CancellationToken ct = default);
    }
}
