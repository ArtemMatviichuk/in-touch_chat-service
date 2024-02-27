using ChatService.Data.Entity;
using ChatService.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Data.Repositories.Implementations
{
    public class ChatRepository : Repository<Chat>, IChatRepository
    {
        public ChatRepository(ChatContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Chat>> GetByUserId(int id)
        {
            return await _context.Set<Chat>()
                .AsNoTracking()
                .Include(e => e.Info)
                .Include(e => e.Participants)
                .Where(e => e.Participants!.Any(p => p.Id == id))
                .ToListAsync();
        }

        public async Task<Chat?> GetFullChat(int id)
        {
            return await _context.Set<Chat>()
                .AsNoTracking()
                .Include(e => e.Info)
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Chat?> GetFullChatAsTracking(int id)
        {
            return await _context.Set<Chat>()
                .Include(e => e.Info)
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
