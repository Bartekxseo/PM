using Microsoft.EntityFrameworkCore;
using PM.Core.Entities;
using System.Linq.Expressions;

namespace PM.DataAccess.Repositories
{
    public class ParkingSpaceRepository(ParkingManagerDbContext dbContext) : IParkingSpaceRepository
    {
        private readonly ParkingManagerDbContext _dbContext = dbContext;
        public async Task AddAsync(ParkingSpace entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<ParkingSpace>().Update(entity);
        }

        public async Task AddAsync(IEnumerable<ParkingSpace> entities, CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<ParkingSpace>().AddRangeAsync(entities, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity != null)
            {
                _dbContext.Set<ParkingSpace>().Remove(entity);
            }
        }

        public async Task<IEnumerable<ParkingSpace>> FindAsync(Expression<Func<ParkingSpace, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<ParkingSpace>().Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ParkingSpace>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<ParkingSpace>().ToListAsync(cancellationToken);
        }

        public async Task<ParkingSpace> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<ParkingSpace>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new Exception("Parking space not found");
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> GetCountAsync(Expression<Func<ParkingSpace, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<ParkingSpace>().CountAsync(predicate, cancellationToken);
        }
    }
}
