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
        return _context.Orders.Where(o => o.OrderId == orderId && o.BusinessId == businessId).Include(o => o.OrderItems).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersWithItemsAsync(int businessId)
    {
        return await _context.Orders.Where(o => o.BusinessId == businessId).Include(o => o.OrderItems).ToListAsync();
    }
}