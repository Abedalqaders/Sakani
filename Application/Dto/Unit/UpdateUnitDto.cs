using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Unit
{
    public class UpdateUnitDto
    {
        public Guid Id { get; set; }
        public string UnitNo { get; set; }
        public string Floor { get; set; }
        public string Area { get; set; }
        public decimal RentPrice { get; set; }
        public Guid PropertyId { get; set; } // في حال أردنا نقل الشقة لعقار آخر (نادرة لكن ممكنة برمجياً)
        public Domain.Enums.UnitStatus UnitStatus { get; set; }
    }
}
