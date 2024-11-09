
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shopipy.UserManagement.Models;

namespace Shopipy.ApiService.Data.Interceptors;

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
}