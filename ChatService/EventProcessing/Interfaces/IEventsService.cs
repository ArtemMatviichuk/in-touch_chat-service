using ChatService.Common.Dtos.MessageBusDtos;

namespace ChatService.EventProcessing.Interfaces
{
    public interface IEventsService
    {
        Task CreateUser(IdPublicIdDto? dto);
        Task ClearUser(IdDto? dto);
        Task ClearNotExistingUsers(IEnumerable<IdDto> dtos);
    }
}
