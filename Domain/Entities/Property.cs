using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Property:BaseEntity
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string AddressRegion { get; set; }
        public string BuildingNo { get; set; }
        public PropertyType PropertyType { get; set; }

        // Navigation Properties
        public ICollection<Unit> Units { get; set; }
    }
}
