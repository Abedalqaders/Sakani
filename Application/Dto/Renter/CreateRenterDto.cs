using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Renter
{
    public class CreateRenterDto
    {
        public string NationalId { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; } // Required if not storing separately
        public string Email { get; set; }    // To create the User account
        public string Description { get; set; }
    }
}
