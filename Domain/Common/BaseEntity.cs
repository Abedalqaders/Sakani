using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TenatId {get; set; }
        public DateTime CreatedAt { set; get; } = DateTime.UtcNow;
        public Guid? CreatedBy { set; get;}
        public DateTime UpdatedAt { set; get; }
        public Guid? UpdatedBy { set; get; }
        public bool IsDeleted { set; get; } = false;

    }
}
