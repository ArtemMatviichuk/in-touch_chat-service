using ChatService.Data.Entity;

namespace ChatService.Data.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByPublicId(string publicId);
        Task<IEnumerable<User>> GetByPublicId(IEnumerable<string> publicIds);
        Task<User?> GetByAuthId(int authId);
        Task<User?> GetByAuthIdAsTracking(int authId);
    }
}