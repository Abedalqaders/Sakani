using Application.Common.Interfaces.Contract;
using Application.Common.Interfaces.General;
using Application.Common.Interfaces.Payment;
using Application.Common.Interfaces.Renter;
using Application.Common.Interfaces.User;
using Application.Dto.Contract;
using Domain.Entities;
using Domain.Enums;


namespace Application.Services
{
    public class ContractAppService : IContractAppService
    {
        
        private readonly IContractRepository _contractRepo;
        private readonly IGenericRepository<Unit> _unitRepo;
        private readonly IRenterRepository _renterRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentRepository _paymentRepo;
        private readonly ICurrentUserService _currentUserService;
        public ContractAppService(
            IContractRepository contractRepo,
            IGenericRepository<Unit> unitRepo,
            IRenterRepository renterRepo,
            IUnitOfWork unitOfWork,
            IPaymentRepository paymentRepo,
           ICurrentUserService currentUserService)
        {
            _contractRepo = contractRepo;
            _unitRepo = unitRepo;
            _renterRepo = renterRepo;
            _unitOfWork = unitOfWork;
            _paymentRepo = paymentRepo;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> CreateContractAsync(CreateContractDto dto, CancellationToken ct)
        {
            var unit = await _unitRepo.GetByIdAsync(dto.UnitId, ct);
            if (unit == null || unit.UnitStatus != UnitStatus.Available)
            {
                throw new InvalidOperationException("Unit not found or not available for rent.");
            }

            var renterExists = await _renterRepo.AnyAsync(r => r.Id == dto.RenterId, ct);
            if (!renterExists)
            {
                throw new KeyNotFoundException("Renter not found.");
            }

            var contract = new Contract
            {
                Id = Guid.NewGuid(),
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                RentAmount = dto.RentAmount,
                UnitId = dto.UnitId,
                RenterId = dto.RenterId,
                PaymentFreq = dto.PaymentFreq,
                ContractStatus = ContractStatus.Active,
                Payments = new List<Payment>()
            };


            GeneratePaymentSchedule(contract, (byte)dto.PaymentFreq);

            unit.UnitStatus = UnitStatus.Rented;
            _unitRepo.Update(unit);

            _contractRepo.Add(contract);
            await _unitOfWork.SaveChangesAsync(ct);

            return contract.Id;
        }
        public async Task<MyContractDetailsDto> GetMyContractAsync(CancellationToken ct)
        {
            var renterid = _currentUserService.RenterId;
            if (renterid == Guid.Empty)
            {
                return new MyContractDetailsDto();
            }
            return await _contractRepo.GetActiveContractForRenterAsync(renterid.Value, ct);

        }
        public async Task<Guid> TerminateContractAsync(Guid contractId, CancellationToken ct)
        {
            var contract = await _contractRepo.GetContractWithUnitAsync(contractId, ct);
            if (contract == null)
            {
                throw new KeyNotFoundException("Contract not found.");
            }

            if (contract.ContractStatus != ContractStatus.Active)
            {
                throw new InvalidOperationException("Only active contracts can be terminated.");
            }

            contract.ContractStatus = ContractStatus.Terminated;
            contract.EndDate = DateTime.UtcNow;
            _contractRepo.Update(contract);

            if (contract.Unit!=null)
            {
                contract.Unit.UnitStatus = UnitStatus.Available;
                _unitRepo.Update(contract.Unit);
            }

            await _paymentRepo.CancelPaymentForContract(contractId, ct);

            await _unitOfWork.SaveChangesAsync(ct);
            return contract.Id;
        }
        private void GeneratePaymentSchedule(Contract contract, byte intervalInMonths)
        {

            int totalMonths = ((contract.EndDate.Year - contract.StartDate.Year) * 12) + contract.EndDate.Month - contract.StartDate.Month;

            int numberOfInstallments = totalMonths / intervalInMonths;

            decimal amountPerInstallment = Math.Round(contract.RentAmount / numberOfInstallments, 2);
            decimal totalAllocated = 0;

            for (int i = 1; i <= numberOfInstallments; i++)
            {
                decimal currentAmount = (i == numberOfInstallments) ? (contract.RentAmount - totalAllocated) : amountPerInstallment;

                contract.Payments.Add(new Payment
                {
                    Id = Guid.NewGuid(),
                    Amount = currentAmount,
                    DueDate = contract.StartDate.AddMonths((i - 1) * intervalInMonths),
                    PaymentStatus = PaymentStatus.Pending,
                    ContractId = contract.Id,
                });

                totalAllocated += currentAmount;
            }
        }

        public async Task<ContractResponseDto?> GetContractWithPaymentsAsync(Guid contractId, CancellationToken ct)
        {
            var Contract = await _contractRepo.GetContractWithPaymentsAsync(contractId, ct);
            if (Contract == null)
            {
                throw new KeyNotFoundException("Contract not found.");
            }
            return new ContractResponseDto
            {
                Id = contractId,
                StartDate = Contract.StartDate,
                EndDate = Contract.EndDate,
                RentAmount = Contract.RentAmount,
                ContractStatus = Contract.ContractStatus,
                UnitId = Contract.UnitId,
                RenterId = Contract.RenterId,
                Payments = Contract.Payments.Select(p => new PaymentResponseDto
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    DueDate = p.DueDate,
                    PaymentDate = p.PaymentDate,
                    PaymentStatus = p.PaymentStatus
                }).OrderBy(p => p.DueDate).ToList()

            };
        }


        public async Task<IReadOnlyList<ContractBasicResponseDto>> GetBasicContractsForTenantAsync(CancellationToken ct)
        {
            var contracts = await _contractRepo.GetBasicContractsForTenantAsync(ct);

            // الميدل وير سيمسك هذا الاستثناء ويحوله لـ 404
            if (contracts == null || !contracts.Any())
            {
                throw new KeyNotFoundException("No contracts found for this tenant.");
            }

            return contracts;
        }

        public async Task<ContractBasicResponseDto> GetActiveContractByUnitId(Guid unitId, CancellationToken ct)
        {
            var contract = await _contractRepo.GetActiveContractsByUnitIdAsync(unitId, ct);

            if (contract == null)
            {
                throw new KeyNotFoundException("No active contract found for this unit.");
            }

            return contract;
        }

    }
}