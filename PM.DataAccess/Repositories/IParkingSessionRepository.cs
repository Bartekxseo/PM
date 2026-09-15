using PM.Core.Entities;
using System.Linq.Expressions;

namespace PM.DataAccess.Repositories
{
    public interface IParkingSessionRepository
    {
        public abstract Task<ParkingSession> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        public abstract Task<IEnumerable<ParkingSession>> GetAllAsync(CancellationToken cancellationToken = default);
        public abstract Task AddAsync(ParkingSession entity, CancellationToken cancellationToken = default);
        public abstract Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        public abstract Task<IEnumerable<ParkingSession>> FindAsync(Expression<Func<ParkingSession, bool>> predicate, CancellationToken cancellationToken = default);
        public Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
