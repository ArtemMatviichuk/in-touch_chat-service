using ChatService.Data.Entity;
using ChatService.Data.Repositories.Interfaces;

namespace ChatService.Data.Repositories.Implementations
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(ChatContext context)
            : base(context)
        {
        }
    }
}
