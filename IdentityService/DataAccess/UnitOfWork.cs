using IdentityService.DataAccess.Context;

namespace IdentityService.DataAccess;

public sealed class UnitOfWork(ApplicationDbContext db)
{
    private int depth;
    private bool rollbackOnly;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);

    public async Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);
        var isOuter = depth == 0;
        var transaction = isOuter
            ? await db.Database.BeginTransactionAsync(cancellationToken)
            : null;
        depth++;

        try
        {
            await action();
            if (isOuter)
            {
                if (rollbackOnly)
                    throw new InvalidOperationException("A nested operation failed. The transaction must roll back.");
                await transaction!.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            rollbackOnly = true;
            if (isOuter)
            {
                try
                {
                    await transaction!.RollbackAsync(CancellationToken.None);
                }
                finally
                {
                    db.ChangeTracker.Clear();
                }
            }
            throw;
        }
        finally
        {
            depth--;
            if (isOuter)
            {
                try
                {
                    await transaction!.DisposeAsync();
                }
                finally
                {
                    rollbackOnly = false;
                }
            }
        }
    }
}
