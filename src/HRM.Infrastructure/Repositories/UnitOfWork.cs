using HRM.Domain.Interfaces;
using HRM.Infrastructure.Data;

namespace HRM.Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation to coordinate repository operations.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly HrmDbContext _context;

    public UnitOfWork(HrmDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
