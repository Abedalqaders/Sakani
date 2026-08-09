using Application.Dto.MaintenanceTicket;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces.General;
namespace Application.Common.Interfaces.MaintenaceTicket
{
    public interface IMaintenanceTicketRepository: IGenericRepository<MaintenanceTicket>
    {

        IQueryable<MaintenanceTicket> GetByRenterIdAsync(Guid renterId);
        Task<IReadOnlyList<MaintenanceTicket>> GetFilteredAsync(TicketFilterDto filter, CancellationToken ct = default);
        Task<List<TicketResponseDto>> GetTicketsByRenterIdAsync(Guid renterId, CancellationToken ct = default);

    }
}
