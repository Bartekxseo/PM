using PM.Core.Entities;
using System.Linq.Expressions;

namespace PM.DataAccess.Repositories
{
    public interface IParkingSpaceRepository
    {
        public Task<ParkingSpace> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        public Task<IEnumerable<ParkingSpace>> GetAllAsync(CancellationToken cancellationToken = default);
        public Task AddAsync(ParkingSpace entity, CancellationToken cancellationToken = default);
        public Task AddAsync(IEnumerable<ParkingSpace> entities, CancellationToken cancellationToken = default);
        public Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        public Task<IEnumerable<ParkingSpace>> FindAsync(Expression<Func<ParkingSpace, bool>> predicate, CancellationToken cancellationToken = default);
        public Task<int> GetCountAsync(Expression<Func<ParkingSpace, bool>> predicate, CancellationToken cancellationToken = default);
        public Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
