using MediatR;
using PM.Core.Entities.Enums;
using PM.DataAccess.Repositories;
using PM.Services.Infra.Dto;
using PM.Services.Infra.Queries;

namespace PM.Services.Handlers.Queries
{
    public class GetParkingSpaceStatsQuerryHandler(IParkingSpaceRepository spaceRepository) : IRequestHandler<GetParkingSpaceStatsQuerry, GetParkingSpaceStatsResult>
    {
        private readonly IParkingSpaceRepository _spaceRepository = spaceRepository;
        public async Task<GetParkingSpaceStatsResult> Handle(GetParkingSpaceStatsQuerry request, CancellationToken cancellationToken)
        {
            return new GetParkingSpaceStatsResult(
                await _spaceRepository.GetCountAsync(x => x.Status == ParkingSpaceStatus.Available, cancellationToken),
                await _spaceRepository.GetCountAsync(x => x.Status == ParkingSpaceStatus.Occupied, cancellationToken)
                );
        }
    }
}
