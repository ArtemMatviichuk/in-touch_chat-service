using ChatService.Common.Dtos.Chat;
using ChatService.Common.Dtos.General;

namespace ChatService.Services.Interfaces
{
    public interface IChatService
    {
        Task<IEnumerable<ChatDto>> GetChats(int authId);
        Task<ChatDto> GetChat(int authId, int chatId);
        Task<ChatDto> CreateChat(int authId, CreateChatDto dto);
        Task UpdateChat(int authId, int chatId, UpdateChatDto dto);
        Task DeleteChat(int authId, int chatId);
        Task LeaveChat(int authId, int chatId);
        Task<FileDto> GetChatImage(int authId, int chatId);
    }
}
