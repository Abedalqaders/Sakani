using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Unit:TenantEntity
    {
        public string UnitNo { get; set; }
        public string Floor { get; set; }
        public string Area { get; set; }
        public decimal RentPrice { get; set; }
        public Guid PropertyId { get; set; }
        public UnitStatus UnitStatus { get; set; }
        public bool IsVacancyNotified { get; set; } = false;

        // Navigation Properties
        public Property Property { get; set; }
        public ICollection<Contract> Contracts { get; set; }
    }
}
