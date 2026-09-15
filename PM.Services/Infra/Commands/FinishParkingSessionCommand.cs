using MediatR;
using PM.Services.Infra.Dto;

namespace PM.Services.Infra.Commands
{
    public record FinishParkingSessionCommand(string VehicleReg) : IRequest<FinishParkingSessionResult>;
}
