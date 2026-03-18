using Application.Dto.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ITenantAppService
    {
      
        Task<IReadOnlyList<TenantResponseDto>> GetAllTenantsAsync(CancellationToken cancellationToken = default);

        Task<TenantResponseDto?> GetTenantByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task DeleteTenantAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateTenantAsync(UpdateTenantDto dto, CancellationToken cancellationToken = default);
        Task<Guid> CreateTenantAsync(CreateTenantDto dto, CancellationToken cancellationToken = default);
    }
}
