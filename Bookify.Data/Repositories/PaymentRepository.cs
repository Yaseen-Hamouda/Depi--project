using Bookify.Core.Interfaces;
using Bookify.Core.Models;
using Bookify.Data.Data;

namespace Bookify.Data.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }

}
