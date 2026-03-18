using Domain.Enums;

public class TenantResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string AddressCity { get; set; }
    public TenantStatus Status { get; set; }
}