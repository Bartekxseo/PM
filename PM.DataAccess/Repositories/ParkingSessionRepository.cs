using Microsoft.EntityFrameworkCore;
using PM.Core.Entities;
using System.Linq.Expressions;

namespace PM.DataAccess.Repositories
{
    public class ParkingSessionRepository(ParkingManagerDbContext dbContext) : IParkingSessionRepository
    {
        private readonly ParkingManagerDbContext _dbContext = dbContext;

        public async Task AddAsync(ParkingSession entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<ParkingSession>().Update(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity != null)
            {
                _dbContext.Set<ParkingSession>().Remove(entity);
            }
        }

        public async Task<IEnumerable<ParkingSession>> FindAsync(Expression<Func<ParkingSession, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<ParkingSession>().Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ParkingSession>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<ParkingSession>().ToListAsync(cancellationToken);
        }

        public async Task<ParkingSession> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<ParkingSession>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new Exception("Parking session not found");
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
