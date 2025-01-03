using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shopipy.Persistence.Models;

namespace Shopipy.Persistence.Data.Interceptors;

public class UpdatedAtInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var context = eventData.Context;
        if (context == null) return result;
        foreach (var entity in context.ChangeTracker.Entries<User>().Where(e => e.State == EntityState.Modified))
        {
            entity.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        return new ValueTask<InterceptionResult<int>>(SavingChanges(eventData, result));
    }
}