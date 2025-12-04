using Bookify.Core.Interfaces;
using Bookify.Core.Models;
using Bookify.Data.Data;

namespace Bookify.Data.Repositories
{
    public class RoomTypeRepository : GenericRepository<RoomType>, IRoomTypeRepository
    {
        public RoomTypeRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }

}
