using ChatService.Data.Entity;
using ChatService.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Data.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ChatContext context)
            : base(context)
        {
        }

        public async Task<User?> GetByPublicId(string publicId)
        {
            return await _context.Set<User>()
                .AsTracking()
                .FirstOrDefaultAsync(e => e.PublicId == publicId);
        }

        public async Task<IEnumerable<User>> GetByPublicId(IEnumerable<string> publicIds)
        {
            return await _context.Set<User>()
                .AsTracking()
                .Where(e => publicIds.Contains(e.PublicId))
                .ToListAsync();
        }

        public async Task<User?> GetByAuthId(int authId)
        {
            return await _context.Set<User>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.AuthenticationId == authId);
        }

        public async Task<User?> GetByAuthIdAsTracking(int authId)
        {
            return await _context.Set<User>()
                .AsTracking()
                .FirstOrDefaultAsync(e => e.AuthenticationId == authId);
        }
    }
}