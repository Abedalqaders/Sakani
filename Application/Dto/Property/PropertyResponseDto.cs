using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Property
{
   public class PropertyResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string AddressRegion { get; set; }
        public string BuildingNo { get; set; }
        public string PropertyType { get; set; }
    }
}
