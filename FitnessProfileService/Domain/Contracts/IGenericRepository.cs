using System.Linq.Expressions;

namespace FitnessProfileService.Domain.Contracts
{
    public interface IGenericRepository<T>
    {
        IQueryable<T> GetAll();
        IQueryable<T> Get(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Update(T entity);
        //void Delete(T entity);
        void Delete(T entity);
        Task<int> CountAsync(CancellationToken cancellationToken, Expression<Func<T, bool>>? criteria = null);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    }
}
