using System.Linq.Expressions;
using IdentityService.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DataAccess.Repositories;

public sealed class GenericRepository<TEntity>(ApplicationDbContext db) : IGenericRepository<TEntity>
    where TEntity : class
{
    public IQueryable<TEntity> GetAll(bool tracking = false) =>
        tracking ? db.Set<TEntity>() : db.Set<TEntity>().AsNoTracking();

    public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate, bool tracking = false) =>
        GetAll(tracking).Where(predicate);

    // FindAsync returns a tracked entity, including entities already loaded in this scope.
    public Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default) =>
        db.Set<TEntity>().FindAsync([id], cancellationToken).AsTask();

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        await db.Set<TEntity>().AddAsync(entity, cancellationToken);

    public void Update(TEntity entity) => db.Set<TEntity>().Update(entity);

    public void Delete(TEntity entity) => db.Set<TEntity>().Remove(entity);

    public void DeleteRange(IEnumerable<TEntity> entities) => db.Set<TEntity>().RemoveRange(entities);

    public Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default) =>
        predicate is null
            ? GetAll().CountAsync(cancellationToken)
            : GetAll().CountAsync(predicate, cancellationToken);
}
