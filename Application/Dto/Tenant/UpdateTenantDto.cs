using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Tenant
{
    public class UpdateTenantDto
    {
      
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressCity { get; set; }
        public string AddressStreet { get; set; }
        public string AddressRegion { get; set; }

  
        public TenantStatus Status { get; set; }
    }
}
