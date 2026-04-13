using Application.Dto.Renter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IRenterAppService
    {
        Task<Guid> CreateAsync(CreateRenterDto dto, CancellationToken ct);
        Task<IReadOnlyList<RenterResponseDto>> GetAllAsync(CancellationToken ct);
        Task<RenterResponseDto?> GetByIdAsync(Guid id, CancellationToken ct);
     
    }
}
