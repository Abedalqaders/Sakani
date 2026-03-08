using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Renter:BaseEntity
    {
        public string NationalId { get; set; }
        public string PhoneNumber { get; set;}
        public  Guid  UserId { set; get; }
        public string Description { get; set; }

        public User User { get; set;}
    }
}
