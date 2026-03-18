using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
namespace Application.Dto.Property
{
    public class UpdatePropertyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string AddressRegion { get; set; }
        public string BuildingNo { get; set; }
        public PropertyType PropertyType { get; set; }
    }
}
