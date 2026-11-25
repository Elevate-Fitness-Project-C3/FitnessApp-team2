using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Storage;
using TrainingService.Infrastructure.Persistence;
using TrainingService.Repositories.Interfaces;

namespace TrainingService.Infrastructure.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TrainingProgramDbContext _context;
        private readonly ConcurrentDictionary<Type, object> _repositories = new();
        private IDbContextTransaction? _transaction;
        private int _depth = 0;

        public UnitOfWork(TrainingProgramDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            return (IGenericRepository<TEntity>)_repositories.GetOrAdd(
                typeof(TEntity),
                _ => new GenericRepository<TEntity>(_context));
        }

        public async Task ExecuteAsync(Func<Task> action)
        {
            var isOuterTransaction = _depth == 0;

            if (isOuterTransaction)
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }

            _depth++;

            try
            {
                await action();

                if (isOuterTransaction)
                {
                    await _context.SaveChangesAsync();
                    await _transaction!.CommitAsync();
                }
            }
            catch
            {
                if (isOuterTransaction && _transaction != null)
                {
                    await _transaction.RollbackAsync();
                }

                throw;
            }
            finally
            {
                _depth--;
                if (isOuterTransaction && _transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            var isOuterTransaction = _depth == 0;

            if (isOuterTransaction)
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }

            _depth++;

            try
            {
                var result = await action();

                if (isOuterTransaction)
                {
                    await _context.SaveChangesAsync();
                    await _transaction!.CommitAsync();
                }

                return result;
            }
            catch
            {
                if (isOuterTransaction && _transaction != null)
                {
                    await _transaction.RollbackAsync();
                }

                throw;
            }
            finally
            {
                _depth--;
                if (isOuterTransaction && _transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task AddSavePointAsync(string name)
        {
            await _context.SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CreateSavepointAsync(name);
            }
        }

        public async Task RollbackToSavePointAsync(string name)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackToSavepointAsync(name);
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
        }
    }
}
