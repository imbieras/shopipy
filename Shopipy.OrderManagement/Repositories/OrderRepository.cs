using Microsoft.EntityFrameworkCore;
using Shopipy.Persistence.Data;
using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;

namespace Shopipy.OrderManagement.Repositories;

public class OrderRepository(AppDbContext context) : GenericRepository<Order>(context)
{
    private readonly AppDbContext _context = context;
    public Task<Order?> GetOrderByIdWithItemsAsync(int businessId, int orderId)
    {
        // I need to remove Orders.OrderDiscounts that have OrderItemId
        return context.Orders.Where(o => o.OrderId == orderId && o.BusinessId == businessId)
            .Include(o => o.OrderDiscounts)
            .Include(o => o.OrderItems)!
            .ThenInclude(i => i.OrderDiscounts)
            .Select(o => RemoveRedundantDiscounts(o))
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersWithItemsAsync(int businessId)
    {
        return await context.Orders.Where(o => o.BusinessId == businessId)
            .Include(o => o.OrderDiscounts)
            .Include(o => o.OrderItems)!
            .ThenInclude(i => i.OrderDiscounts)
            .Select(o => RemoveRedundantDiscounts(o))
            .ToListAsync();
    }

    private static Order RemoveRedundantDiscounts(Order order)
    {
        var newOrder = order.Clone();
        newOrder.OrderDiscounts = newOrder.OrderDiscounts?.Where(d => d.OrderItemId == null);
        return newOrder;
    }
}