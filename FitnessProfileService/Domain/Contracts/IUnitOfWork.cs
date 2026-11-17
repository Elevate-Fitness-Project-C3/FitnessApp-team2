namespace FitnessProfileService.Domain.Contracts
{
    internal interface IUnitOfWork
    {
         Task ExcuteAsync(Func<Task> action);
        Task AddSavePointAsync(string savePoint);
        Task RollbackToSavePointAsync(string name);
    }
}
