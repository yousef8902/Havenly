namespace Havenly.DAL.Repos.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        IBookingRepository Bookings { get; }
        Task<int> SaveChangesAsync();
    }
}
