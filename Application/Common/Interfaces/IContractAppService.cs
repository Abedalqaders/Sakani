using Application.Dto.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IContractAppService
    {
       public Task<Guid> CreateContractAsync(CreateContractDto dto, CancellationToken ct);

        public Task<ContractResponseDto?> GetContractWithPaymentsAsync(Guid contractId, CancellationToken ct);

        public Task<IReadOnlyList<ContractBasicResponseDto?>> GetBasicContractsForTenantAsync(CancellationToken ct);

        public Task<ContractBasicResponseDto> GetActiveContractByUnitId(Guid unitId, CancellationToken ct);

        public Task<Guid> TerminateContractAsync(Guid contractId, CancellationToken ct);



    }
}
