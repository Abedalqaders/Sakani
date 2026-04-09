using Application.Dto.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Unit
{
    public interface IUnitAppService
    {
        Task<IReadOnlyList<UnitResponseDto>> GetAllAsync(CancellationToken ct);
        Task<IReadOnlyList<UnitResponseDto>> GetUnitsByPropertyIdAsync(Guid propertyId, CancellationToken ct);
        Task<UnitResponseDto> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Guid> CreateAsync(CreateUnitDto dto, CancellationToken ct);
        Task UpdateAsync(UpdateUnitDto dto, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);

    }
}
