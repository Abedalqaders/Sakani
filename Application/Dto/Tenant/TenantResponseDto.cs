using Domain.Enums;

public class TenantResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string AddressCity { get; set; }
    public string AddressStreet { get; set; }
    public string AddressRegion { get; set; }
    public string Status { get; set; }
}