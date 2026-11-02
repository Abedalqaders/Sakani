using Application.Common.Interfaces.Contract;
using Application.Dto.Contract;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ContractRepository : GenericRepository<Contract>, IContractRepository
    {
        public ContractRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ContractResponseDto?> GetContractWithPaymentsAsync(Guid contractId, CancellationToken ct)
        {
            return await _context.Set<Contract>()
                .AsNoTracking() 
                .Where(c => c.Id == contractId)
                .Select(c => new ContractResponseDto
                {
                    Id = c.Id,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    RentAmount = c.RentAmount,
                    ContractStatus = c.ContractStatus,
                    UnitId = c.UnitId,
                    RenterId = c.RenterId,
                   
                    Payments = c.Payments.Select(p => new PaymentResponseDto
                    {
                        Id = p.Id,
                        Amount = p.Amount,
                        DueDate = p.DueDate,
                        PaymentDate = p.PaymentDate,
                        PaymentStatus = p.PaymentStatus
                    }).OrderBy(p => p.DueDate).ToList()
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<ContractBasicResponseDto?> GetActiveContractsByUnitIdAsync(Guid unitId, CancellationToken ct)
        {
            return await _context.Set<Contract>()
                .AsNoTracking()
                .Where(c => c.UnitId == unitId && c.ContractStatus == ContractStatus.Active).Select(c => new ContractBasicResponseDto
                {
                    Id = c.Id,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    RentAmount = c.RentAmount,
                    ContractStatus = c.ContractStatus,
                    UnitId = c.UnitId,
                    RenterId = c.RenterId
                })
                .FirstOrDefaultAsync(ct);
        }
        public async Task<IReadOnlyList<MyContractDetailsDto?>> GetActiveContractForRenterAsync(Guid renterId, CancellationToken ct)
        {
            return await _context.Set<Contract>()
        .AsNoTracking().Where(c => c.RenterId == renterId && c.ContractStatus == ContractStatus.Active).Select(c => new MyContractDetailsDto
        {
            ContractId = c.Id,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            RentAmount = c.RentAmount,
            ContractStatus = c.ContractStatus,
            UnitNo = c.Unit.UnitNo,
            PropertyName = c.Unit.Property.Name
        }).ToListAsync(ct);

        }
        public async Task<IReadOnlyList<ContractBasicResponseDto>> GetBasicContractsForTenantAsync(CancellationToken ct)
        { 
          return await _context.Set<Contract>()
                .AsNoTracking()
                .Select(c => new ContractBasicResponseDto
                {
                    Id = c.Id,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    RentAmount = c.RentAmount,
                    ContractStatus = c.ContractStatus,
                    UnitId = c.UnitId,
                    RenterId = c.RenterId
                })
                .ToListAsync(ct);
        }
        public async Task<Contract?> GetContractWithUnitAsync(Guid contractId, CancellationToken ct)
        {
            return await _context.Set<Contract>()
                                 .Include(c => c.Unit)
                                 .FirstOrDefaultAsync(c => c.Id == contractId, ct);
        }
    }
}
