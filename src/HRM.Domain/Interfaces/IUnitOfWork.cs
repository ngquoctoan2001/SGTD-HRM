namespace HRM.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern to coordinate multiple repository operations.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync();
}
