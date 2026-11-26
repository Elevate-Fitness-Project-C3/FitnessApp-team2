using TrainingService.Repositories.Interfaces;

namespace TrainingService.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;

        Task ExecuteAsync(Func<Task> action);

        Task<T> ExecuteAsync<T>(Func<Task<T>> action);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task AddSavePointAsync(string name);

        Task RollbackToSavePointAsync(string name);
    }
}
