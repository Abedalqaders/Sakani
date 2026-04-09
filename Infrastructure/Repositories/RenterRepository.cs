using Application.Common.Interfaces.Renter;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class RenterRepository:GenericRepository<Renter>,IRenterRepository
    {
        public RenterRepository(ApplicationDbContext Context):base(Context) {
        }

    }
}
