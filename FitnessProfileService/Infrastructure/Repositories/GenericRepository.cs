using FitnessProfileService.Domain.Contracts;
using FitnessProfileService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FitnessProfileService.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ProfileDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ProfileDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }



        public IQueryable<T> GetAll()
        {
            return _dbSet;
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }


        //// Soft Delete - marks as deleted but keeps in database
        //public void Delete(T entity)
        //{
        //    _dbSet.Attach(entity);
        //    entity.IsDeleted = true;
        //    entity.DeletedAt = DateTime.UtcNow;

        //    // Mark the entity as modified so EF will update it
        //    _context.Entry(entity).State = EntityState.Modified;
        //}

        // Hard Delete - physically removes from database
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _context.RemoveRange(entities);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken, Expression<Func<T, bool>>? criteria = null)
        {
            if (criteria == null)
            {
                return await _dbSet.CountAsync(cancellationToken);
            }

            return await _dbSet.CountAsync(criteria, cancellationToken);
        }
    }
    }
