using ChatService.Data.Entity;

namespace ChatService.Data.Repositories.Interfaces
{
    public interface IChatRepository : IRepository<Chat>
    {
        Task<IEnumerable<Chat>> GetByUserId(int id);
        Task<Chat?> GetFullChat(int id);
        Task<Chat?> GetFullChatAsTracking(int id);
    }
}
