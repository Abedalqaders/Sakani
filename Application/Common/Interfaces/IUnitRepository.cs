using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IUnitRepository: IGenericRepository<Unit>
    {
        Task<IReadOnlyList<Unit>> GetAllWithPropertyAsync(CancellationToken ct);
        Task<Unit?> GetByIdWithPropertyAsync(Guid id, CancellationToken ct);
        Task<IReadOnlyList<Unit>> GetUnitsByPropertyAsync(Guid propertyId, CancellationToken ct);
        Task<decimal> GetOccupancyRateAsync(CancellationToken ct);

    }
}
