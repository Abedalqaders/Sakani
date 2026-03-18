using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Dto.Tenant
{
    public class CreateTenantDto
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
