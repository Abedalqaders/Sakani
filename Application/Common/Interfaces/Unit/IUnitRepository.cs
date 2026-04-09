using Application.Common.Interfaces.General;
using DomainUnit = Domain.Entities.Unit;

namespace Application.Common.Interfaces.Unit
{
    public interface IUnitRepository: IGenericRepository<DomainUnit>
    {
        Task<IReadOnlyList<DomainUnit>> GetAllWithPropertyAsync(CancellationToken ct);
        Task<DomainUnit?> GetByIdWithPropertyAsync(Guid id, CancellationToken ct);
        Task<IReadOnlyList<DomainUnit>> GetUnitsByPropertyAsync(Guid propertyId, CancellationToken ct);
        Task<decimal> GetOccupancyRateAsync(CancellationToken ct);
    }
}
