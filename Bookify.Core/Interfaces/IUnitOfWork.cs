namespace Bookify.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRoomTypeRepository RoomTypes { get; }
        IRoomRepository Rooms { get; }
        IBookingRepository Bookings { get; }
        IPaymentRepository Payments { get; }

        Task<int> CompleteAsync();
    }
}
