using Application.Dto.Contract;
using Domain.Entities;


namespace Application.Common.Interfaces
{
    public interface IContractRepository : IGenericRepository<Contract>
    {
        Task<ContractResponseDto?> GetContractWithPaymentsAsync(Guid contractId, CancellationToken ct);
        Task<ContractBasicResponseDto?> GetActiveContractsByUnitIdAsync(Guid unitId, CancellationToken ct);
        Task<IReadOnlyList<ContractBasicResponseDto?>> GetBasicContractsForTenantAsync(CancellationToken ct);
        public Task<Contract?> GetContractWithUnitAsync(Guid contractId, CancellationToken ct);



    }
}
