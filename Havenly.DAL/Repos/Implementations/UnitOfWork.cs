using Havenly.DAL.Database;
using Havenly.DAL.Repos.Abstractions;


namespace Havenly.DAL.Repos.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HavenlyDbContext _context;
        public IBookingRepository Bookings { get; }

        public UnitOfWork(HavenlyDbContext context, IBookingRepository bookingRepository)
        {
            _context = context;
            Bookings = bookingRepository;
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
}
