using Application.Common.Interfaces;
using Application.Dto.Contract;
using Application.Dto.Renter;
using Domain.Entities;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RenterAppService:IRenterAppService
    {
        private readonly IRenterRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public RenterAppService(IRenterRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateAsync(CreateRenterDto dto, CancellationToken ct)
        {
            var exists = await _repository.AnyAsync(r => r.NationalId == dto.NationalId, ct);
            if (exists)
                throw new InvalidOperationException("A renter with this National ID already exists in your records.");

            // 2. Map DTO to Entity
            var renter = new Renter
            {
                Id = Guid.NewGuid(),
                NationalId = dto.NationalId,
                PhoneNumber = dto.PhoneNumber,
                Description = dto.Description ?? string.Empty,
                // UserId is null for now until you integrate Identity
            };

            _repository.Add(renter);

            // 3. Save to DB
            await _unitOfWork.SaveChangesAsync(ct);

            return renter.Id;
        }
        public async Task<IReadOnlyList<RenterResponseDto>> GetAllAsync(CancellationToken ct) {
        
            var renters =await _repository.GetAllAsync(ct);
            return renters.Select(r => new RenterResponseDto
            {
                Id = r.Id,
                NationalId = r.NationalId,
                PhoneNumber = r.PhoneNumber,
                Description = r.Description,
                UserId = r.UserId
            }).ToList();
        }
        public async Task<RenterResponseDto?> GetByIdAsync(Guid id, CancellationToken ct) { 
      var r = await _repository.GetByIdAsync(id, ct);
            if (r == null)
                return null;
            return new RenterResponseDto
            {
                Id = r.Id,
                NationalId = r.NationalId,
                PhoneNumber = r.PhoneNumber,
                Description = r.Description,
                UserId = r.UserId
            };
        }
    }
}
