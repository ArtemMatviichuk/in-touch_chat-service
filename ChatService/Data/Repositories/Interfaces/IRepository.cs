using System.Linq.Expressions;

namespace ChatService.Data.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllAsTracking();
        Task<IEnumerable<T>> GetAllAsTracking(Expression<Func<T, bool>> predicate);

        Task<bool> Exists(int id);
        Task<bool> Exists(Expression<Func<T, bool>> predicate);

        Task<T?> Get(int id);
        Task<T?> Get(Expression<Func<T, bool>> predicate);
        Task<T?> GetAsTracking(int id);
        Task<T?> GetAsTracking(Expression<Func<T, bool>> predicate);

        Task Add(T entity);
        Task AddRange(IEnumerable<T> entities);

        void Remove(T entity);
        Task Remove(int id);
        void RemoveRange(IEnumerable<T> entities);

        Task SaveChanges();
    }
}