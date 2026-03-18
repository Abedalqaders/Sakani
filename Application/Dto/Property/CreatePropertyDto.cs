using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Property
{
    public class CreatePropertyDto
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string AddressRegion { get; set; }
        public string BuildingNo { get; set; }
        public PropertyType PropertyType { get; set; }
        public Guid? TenantId { get; set; }
    }
}
