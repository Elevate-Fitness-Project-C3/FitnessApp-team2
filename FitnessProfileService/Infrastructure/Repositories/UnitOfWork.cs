using FitnessProfileService.Domain.Contracts;
using FitnessProfileService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace FitnessProfileService.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProfileDbContext _dbContext;
        private IDbContextTransaction? _transaction;
        private int _depth = 0;
        private string? _currentSavePoint;

        public UnitOfWork(ProfileDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddSavePointAsync(string savePoint)
        {
            if (_transaction is null)
                throw new InvalidOperationException("No Active Transaction To Add a SavePoint");
            await _transaction.CreateSavepointAsync(savePoint);
            _currentSavePoint = savePoint;
        }

        public async Task ExcuteAsync(Func<Task> action)
        {
            bool isOuterTran = _depth == 0;
            if (isOuterTran)
                _transaction = await _dbContext.Database.BeginTransactionAsync();
            _depth++;
            try
            {
                await action();
                if (isOuterTran)
                {
                    await _dbContext.SaveChangesAsync();
                    await _transaction.CommitAsync();
                }

            }
            catch (Exception)
            {
                if (isOuterTran && _transaction != null)
                {

                    if (string.IsNullOrEmpty(_currentSavePoint))
                        await _transaction.RollbackAsync();
                    else
                        await _transaction.RollbackToSavepointAsync(_currentSavePoint);


                }
                throw;
            }
            finally
            {
                _depth--;
                if (isOuterTran && _transaction is not null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public Task RollbackToSavePointAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
