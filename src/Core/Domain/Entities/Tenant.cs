using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; }
        public string AddressCity { get; set; }
        public string AddressStreet { get; set; }
        public string AddressRegion { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public TenantStatus Status { get; set; } = TenantStatus.Active;
    }
}
