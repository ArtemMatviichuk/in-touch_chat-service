using ChatService.Common.Dtos.MessageBusDtos;

namespace ChatService.SyncDataServices.Grpc
{
    public interface IAuthenticationDataClient
    {
        Task<IEnumerable<IdPublicIdDto>?> GetAllUsers();
    }
}