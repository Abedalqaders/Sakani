using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Unit
{
    public class CreateUnitDto
    {
        public string UnitNo { get; set; }
        public string Floor { get; set; }
        public string Area { get; set; }
        public decimal RentPrice { get; set; }
        public Guid PropertyId { get; set; } 
        public Domain.Enums.UnitStatus UnitStatus { get; set; }
    }
}
