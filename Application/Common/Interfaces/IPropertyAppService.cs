using Application.Dto.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IPropertyAppService
    {

        Task<IReadOnlyList<PropertyResponseDto>> GetAllPropertiesAsync(CancellationToken cancellationToken);
        Task<PropertyResponseDto> GetPropertyByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Guid> CreatePropertyAsync(CreatePropertyDto dto, CancellationToken cancellationToken);
        Task UpdatePropertyAsync(UpdatePropertyDto dto, CancellationToken cancellationToken);
        Task DeletePropertyAsync(Guid id, CancellationToken cancellationToken);
    }
}
