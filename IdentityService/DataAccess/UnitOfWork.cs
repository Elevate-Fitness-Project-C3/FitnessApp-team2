using IdentityService.Common.ResultPattern;
using IdentityService.DataAccess.Context;

namespace IdentityService.DataAccess;

public sealed class UnitOfWork(ApplicationDbContext db)
{
    private int depth;
    private bool rollbackOnly;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);

    public async Task<RequestResponse<T>> ExecuteAsync<T>(Func<Task<RequestResponse<T>>> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);
        var isOuter = depth == 0;
        var transaction = isOuter
            ? await db.Database.BeginTransactionAsync(cancellationToken)
            : null;
        depth++;

        try
        {
            var result = await action();
            rollbackOnly |= !result.Success;
            if (isOuter && rollbackOnly && result.Success)
                return RequestResponse<T>.Fail("A nested operation failed.", StatusCodes.Status500InternalServerError);

            if (isOuter && !rollbackOnly)
                await transaction!.CommitAsync(cancellationToken);

            return result;
        }
        catch
        {
            rollbackOnly = true;
            throw;
        }
        finally
        {
            depth--;
            if (isOuter)
            {
                try
                {
                    if (rollbackOnly)
                        await transaction!.RollbackAsync(CancellationToken.None);
                }
                finally
                {
                    if (rollbackOnly)
                        db.ChangeTracker.Clear();
                    rollbackOnly = false;
                    await transaction!.DisposeAsync();
                }
            }
        }
    }
}
