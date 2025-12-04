using Bookify.Core.Interfaces;
using Bookify.Core.Models;
using Bookify.Data.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bookify.Data.Repositories
{
    public class RoomRepository : GenericRepository<Room>, IRoomRepository
    {
        public RoomRepository(ApplicationDbContext context) : base(context)
        {
        }

        // override to include the RoomType navigation property
        public override async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _context.Rooms
                                 .Include(r => r.RoomType)
                                 .ToListAsync();
        }

        public override async Task<Room> GetByIdAsync(int id)
        {
            return await _context.Rooms
                                 .Include(r => r.RoomType)
                                 .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
