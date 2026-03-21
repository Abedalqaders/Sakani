using Application.Common.Interfaces;
using System;
using Application.Dto.Property;
using Domain.Entities;
namespace Application.Services
{
    public class PropertyAppService : IPropertyAppService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PropertyAppService(IPropertyRepository propertyRepository, IUnitOfWork unitOfWork)
        {
            _propertyRepository = propertyRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<IReadOnlyList<PropertyResponseDto>> GetAllPropertiesAsync(CancellationToken cancellationToken)
        {
            var properties = await _propertyRepository.GetAllAsync(cancellationToken);
            return properties.Select(p => new PropertyResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                City = p.City,
                Street = p.Street,
                AddressRegion = p.AddressRegion,
                BuildingNo = p.BuildingNo,
                PropertyType = p.PropertyType.ToString()
            }).ToList();
        }
        public async Task<PropertyResponseDto> GetPropertyByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var property = await _propertyRepository.GetByIdAsync(id, cancellationToken);
            if (property is null) return null;

            return new PropertyResponseDto
            {
                Id = property.Id,
                Name = property.Name,
                City = property.City,
                Street = property.Street,
                AddressRegion = property.AddressRegion,
                BuildingNo = property.BuildingNo,
                PropertyType = property.PropertyType.ToString()
            };
        }

        public async Task<Guid> CreatePropertyAsync(CreatePropertyDto dto, CancellationToken cancellationToken)
        {
            var property = new Property
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                City = dto.City,
                Street = dto.Street,
                AddressRegion = dto.AddressRegion,
                BuildingNo = dto.BuildingNo,
                PropertyType = dto.PropertyType,
                CreatedAt = DateTime.UtcNow
            };
           

            _propertyRepository.Add(property);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return property.Id;
        }

        public async Task UpdatePropertyAsync(UpdatePropertyDto dto, CancellationToken cancellationToken)
        {
            var property = await _propertyRepository.GetByIdAsync(dto.Id, cancellationToken);

            if (property is null)
                throw new KeyNotFoundException($"Property with ID {dto.Id} was not found.");

            property.Name = dto.Name;
            property.City = dto.City;
            property.Street = dto.Street;
            property.AddressRegion = dto.AddressRegion;
            property.BuildingNo = dto.BuildingNo;
            property.PropertyType = dto.PropertyType;
            property.UpdatedAt = DateTime.UtcNow;

            _propertyRepository.Update(property);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeletePropertyAsync(Guid id, CancellationToken cancellationToken)
        {
            var property = await _propertyRepository.GetByIdAsync(id, cancellationToken);

            if (property is null)
                throw new KeyNotFoundException($"Property with ID {id} was not found.");

            _propertyRepository.Delete(property);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
