using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class User:BaseEntity
    {
        public string Name { get; set; }
        public string PasswordHashed { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public DateTime? LastLogin { set; get; }

        public Role  Role { get; set; }
    }
}
