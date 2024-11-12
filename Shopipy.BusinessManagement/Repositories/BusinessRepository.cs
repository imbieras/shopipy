using Microsoft.EntityFrameworkCore;
using Shopipy.BusinessManagement.Repositories;
using Shopipy.Persistance.Data;
using Shopipy.Persistance.Models;

namespace Shopipy.BusinessManagement.Repositories;

public class BusinessRepository : IBusinessRepository
{
    private readonly AppDbContext _context;

    public BusinessRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Business>> GetAllBusinessesAsync()
    {
        return await _context.Businesses.ToListAsync();
    }

    public async Task<Business> GetBusinessByIdAsync(int id)
    {
        return await _context.Businesses.FindAsync(id);
    }

    public async Task<Business> AddBusinessAsync(Business business)
    {
        _context.Businesses.Add(business);
        await _context.SaveChangesAsync();
        return business;
    }

    public async Task<Business> UpdateBusinessAsync(Business business)
    {
        _context.Businesses.Update(business);
        await _context.SaveChangesAsync();
        return business;
    }

    public async Task<bool> DeleteBusinessAsync(int id)
    {
        var business = await _context.Businesses.FindAsync(id);
        if (business == null) return false;

        _context.Businesses.Remove(business);
        await _context.SaveChangesAsync();
        return true;
    }
}