using Bookify.Core.Interfaces;
using Bookify.Data.Data;

namespace Bookify.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IRoomTypeRepository RoomTypes { get; private set; }
        public IRoomRepository Rooms { get; private set; }
        public IBookingRepository Bookings { get; private set; }
        public IPaymentRepository Payments { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            // إنشاء الريبوستوري هنا وليس في الـ DI
            RoomTypes = new RoomTypeRepository(context);
            Rooms = new RoomRepository(context);
            Bookings = new BookingRepository(context);
            Payments = new PaymentRepository(context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
