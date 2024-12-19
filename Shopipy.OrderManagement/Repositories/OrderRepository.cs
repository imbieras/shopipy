using Microsoft.EntityFrameworkCore;
using Shopipy.Persistence.Data;
using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;

namespace Shopipy.OrderManagement.Repositories;

public class OrderRepository(AppDbContext context) : GenericRepository<Order>(context)
{
    public async Task<Order?> GetOrderByIdWithItemsAsync(int businessId, int orderId)
    {
        // I need to remove Orders.OrderDiscounts that have OrderItemId
        var order = await context.Orders.Where(o => o.OrderId == orderId && o.BusinessId == businessId)
            .Include(o => o.OrderDiscounts)
            .Include(o => o.OrderItems)!
            .ThenInclude(i => i.OrderDiscounts)
            .FirstOrDefaultAsync();

        return order == null ? null : RemoveRedundantDiscounts(order);
    }

    public async Task<IEnumerable<Order>> GetOrdersWithItemsAsync(int businessId)
    {
        var orders = await context.Orders.Where(o => o.BusinessId == businessId)
            .Include(o => o.OrderDiscounts)
            .Include(o => o.OrderItems)!
            .ThenInclude(i => i.OrderDiscounts)
            .ToListAsync();

        foreach (var order in orders)
        {
            RemoveRedundantDiscounts(order);
        }
        return orders;
    }

    private static Order RemoveRedundantDiscounts(Order order)
    {
        // Convert the filtered result back to a List
        order.OrderDiscounts = order.OrderDiscounts?
            .Where(d => d.OrderItemId == null)
            .ToList();
        return order;
    }
}