using Application.Common.Interfaces.General;
using Application.Common.Interfaces.Property;
using Application.Common.Interfaces.Unit;
using Application.Common.Interfaces.User;
using Application.Dto.Unit;
using Domain.Entities;


namespace Application.Services
{
    public class UnitAppService:IUnitAppService
    {
        private readonly IUnitRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPropertyRepository _propertyRepo;
        private readonly ICurrentUserService _currentUserService;
        public UnitAppService(IUnitRepository unitRepository, IUnitOfWork unitOfWork, IPropertyRepository propertyRepo,ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _repo = unitRepository;
            _unitOfWork = unitOfWork;
            _propertyRepo = propertyRepo;
        }
        public async Task<Guid> GetUnitIdByRenter(CancellationToken ct)
        {
            var renterid = _currentUserService.RenterId.Value;
            if (renterid == Guid.Empty)
            {
                return Guid.Empty;
            }
            return await _repo.GetUnitIdByRenter(renterid, ct);
        }
        public async Task<IReadOnlyList<UnitResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
          
            var units = await _repo.GetAllWithPropertyAsync(cancellationToken);

            return units.Select(u => new UnitResponseDto
            {
                Id = u.Id,
                UnitNo = u.UnitNo,
                Floor = u.Floor,
                Area = u.Area,
                RentPrice = u.RentPrice,
                UnitStatus = u.UnitStatus,
                PropertyId = u.PropertyId,
                PropertyName = u.Property?.Name
            }).ToList();
        }
        public async Task<IReadOnlyList<UnitResponseDto>> GetUnitsByPropertyIdAsync(Guid propertyId, CancellationToken ct)
        {
       
            var units = await _repo.GetUnitsByPropertyAsync(propertyId, ct);

      
            return units.Select(u => new UnitResponseDto
            {
                Id = u.Id,
                UnitNo = u.UnitNo,
                Floor = u.Floor,
                Area = u.Area,
                RentPrice = u.RentPrice,
                UnitStatus = u.UnitStatus,
                PropertyId = u.PropertyId,
                PropertyName = u.Property?.Name
            }).ToList();
        }
        public async Task<UnitResponseDto> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var unit = await _repo.GetByIdWithPropertyAsync(id, ct);
            if (unit == null) throw new KeyNotFoundException("Unit not found");
            return new UnitResponseDto
            {
                Id = unit.Id,
                UnitNo = unit.UnitNo,
                Floor = unit.Floor,
                Area = unit.Area,
                RentPrice = unit.RentPrice,
                UnitStatus = unit.UnitStatus,
                PropertyId = unit.PropertyId,
                PropertyName = unit.Property?.Name
            };
        }
        public async Task<Guid> CreateAsync(CreateUnitDto dto, CancellationToken cancellationToken = default)
        {
            var propertyExists = await _propertyRepo.AnyAsync(p => p.Id == dto.PropertyId, cancellationToken);

            if (!propertyExists)
            {
                throw new KeyNotFoundException("The requested property was not found.");
            }
            var unit = new Unit
            {
                Id = Guid.NewGuid(), // ضمان توليد الـ ID
                UnitNo = dto.UnitNo,
                Floor = dto.Floor,
                Area = dto.Area,
                RentPrice = dto.RentPrice,
                PropertyId = dto.PropertyId,
                UnitStatus = dto.UnitStatus
            };

             _repo.Add(unit);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return unit.Id;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var unit = await _repo.GetByIdAsync(id, cancellationToken);

            if (unit == null)
                throw new KeyNotFoundException($"Unit with ID {id} was not found.");

            _repo.Delete(unit);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(UpdateUnitDto dto, CancellationToken cancellationToken = default)
        {
            var unit = await _repo.GetByIdAsync(dto.Id, cancellationToken);

            if (unit is null)
            {
                throw new KeyNotFoundException($"Unit with ID {dto.Id} was not found.");
            }
            if (unit.PropertyId != dto.PropertyId)
            {
                var propertyExists = await _propertyRepo.AnyAsync(p => p.Id == dto.PropertyId, cancellationToken);
                if (!propertyExists)
                {
                    throw new KeyNotFoundException("The requested property was not found.");
                }
            }
            unit.UnitNo = dto.UnitNo;
            unit.Floor = dto.Floor;
            unit.Area = dto.Area;
            unit.RentPrice = dto.RentPrice;
            unit.PropertyId = dto.PropertyId;
            unit.UnitStatus = dto.UnitStatus;
            
            if (unit.UnitStatus == Domain.Enums.UnitStatus.Rented || unit.UnitStatus == Domain.Enums.UnitStatus.Rented)
            {
                unit.IsVacancyNotified = false; 
            }

        
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

    }
    }

