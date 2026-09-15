using MediatR;
using PM.Services.Infra.Dto;

namespace PM.Services.Infra.Queries
{
    public record GetParkingSpaceStatsQuerry() : IRequest<GetParkingSpaceStatsResult>;
}
