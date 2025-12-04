using Bookify.Core.Interfaces;
using Bookify.Core.Models;
using Bookify.Data.Data;

namespace Bookify.Data.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }

}
