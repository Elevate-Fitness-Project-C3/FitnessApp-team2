using System.Linq.Expressions;

namespace TrainingService.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);

        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default);
        IQueryable<TEntity> GetAll(bool tracking = false);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> perdicate, CancellationToken ct = default);
        Task<TEntity?> FirstOrDefualtAsyc(Expression<Func<TEntity, bool>> predicate, bool tracking = false, CancellationToken ct = default);

        Task<int> CountAsync(Expression<Func<TEntity, bool>>? condition = null, CancellationToken ct = default);

        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
