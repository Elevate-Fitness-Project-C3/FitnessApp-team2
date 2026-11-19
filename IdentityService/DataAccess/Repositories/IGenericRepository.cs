using System.Linq.Expressions;

namespace IdentityService.DataAccess.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> GetAll(bool tracking = false);
    IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate, bool tracking = false);
    Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    void DeleteRange(IEnumerable<TEntity> entities);
    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
}
