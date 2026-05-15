using Application.Common.Interfaces.General;
using Application.Dto.Contract;
using Domain.Entities; // Ensure this contains the Contract class, not just a namespace

namespace Application.Common.Interfaces.Contract
{
    public interface IContractRepository : IGenericRepository<Domain.Entities.Contract>
    {
        Task<ContractResponseDto?> GetContractWithPaymentsAsync(Guid contractId, CancellationToken ct);
        Task<ContractBasicResponseDto?> GetActiveContractsByUnitIdAsync(Guid unitId, CancellationToken ct);
        Task<IReadOnlyList<ContractBasicResponseDto?>> GetBasicContractsForTenantAsync(CancellationToken ct);
        Task<Domain.Entities.Contract?> GetContractWithUnitAsync(Guid contractId, CancellationToken ct);
     Task<MyContractDetailsDto?> GetActiveContractForRenterAsync(Guid renterid, CancellationToken ct);
    }
}
