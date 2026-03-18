using Application.Common.Interfaces;
using Application.Dto.Tenant;
using Sakani.Application.Common.Interfaces;


namespace Sakani.Application.Services;

public class TenantAppService : ITenantAppService
{
    private readonly ITenantRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public TenantAppService(ITenantRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<TenantResponseDto>> GetAllTenantsAsync(CancellationToken cancellationToken = default)
    {
        var tenants = await _repo.GetAllAsync(cancellationToken);

      
        return tenants.Select(t => new TenantResponseDto
        {
            Id = t.Id,
            Name = t.Name,
            Email = t.Email,
            PhoneNumber = t.PhoneNumber,
            AddressCity = t.AddressCity,
            Status = t.Status
        }).ToList();
    }
    public async Task<TenantResponseDto?> GetTenantByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tenant = await _repo.GetByIdAsync(id, cancellationToken);

        if (tenant is null) return null;

        return new TenantResponseDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Email = tenant.Email,
            PhoneNumber = tenant.PhoneNumber,
            AddressCity = tenant.AddressCity,
            Status = tenant.Status
        };
    }

    public async Task<Guid> CreateTenantAsync(CreateTenantDto dto, CancellationToken cancellationToken = default)
    {
        var tenant = new Tenant
        {
    
            Name = dto.Name,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            AddressCity = dto.AddressCity,
            AddressStreet = dto.AddressStreet,
            AddressRegion = dto.AddressRegion,
            Status = dto.Status 
        };

        _repo.Add(tenant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return tenant.Id;
    }
    public async Task DeleteTenantAsync(Guid id, CancellationToken cancellationToken = default)
    {
      
        var tenant = await _repo.GetByIdAsync(id, cancellationToken);

        if (tenant == null)
            throw new KeyNotFoundException($"Tenant with ID {id} was not found.");


        _repo.Delete(tenant);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    public async Task UpdateTenantAsync(UpdateTenantDto dto, CancellationToken cancellationToken = default)
    {
      
        var tenant = await _repo.GetByIdAsync(dto.Id, cancellationToken);

        if (tenant is null)
        {
          
            throw new KeyNotFoundException($"Tenant with ID {dto.Id} was not found.");
        }

   
        tenant.Name = dto.Name;
        tenant.Email = dto.Email;
        tenant.PhoneNumber = dto.PhoneNumber;
        tenant.AddressCity = dto.AddressCity;
        tenant.AddressStreet = dto.AddressStreet;
        tenant.AddressRegion = dto.AddressRegion;
        tenant.Status = dto.Status;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}