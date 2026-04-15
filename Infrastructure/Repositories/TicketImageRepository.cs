using Application.Common.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TicketImageRepository : GenericRepository<TicketImage>, ITicketImageRepository
    {
        public TicketImageRepository(ApplicationDbContext context) : base(context)
        {
            // الـ context تم تمريره للـ base class (GenericRepository)
        }
    }
}
