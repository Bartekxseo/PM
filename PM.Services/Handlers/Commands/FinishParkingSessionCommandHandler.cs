using MediatR;
using PM.Core.Entities.Enums;
using PM.DataAccess.Repositories;
using PM.Services.Infra.Commands;
using PM.Services.Infra.Dto;

namespace PM.Services.Handlers.Commands
{
    public class FinishParkingSessionCommandHandler(IParkingSpaceRepository spaceRepository, IParkingSessionRepository sessionRepository) : IRequestHandler<FinishParkingSessionCommand, FinishParkingSessionResult>
    {
        private readonly IParkingSpaceRepository _spaceRepository = spaceRepository;
        private readonly IParkingSessionRepository _sessionRepository = sessionRepository;
        public async Task<FinishParkingSessionResult> Handle(FinishParkingSessionCommand request, CancellationToken cancellationToken)
        {
            var session = (await _sessionRepository.FindAsync(x => x.Registration == request.VehicleReg && !x.TimeOut.HasValue, cancellationToken)).FirstOrDefault() ?? throw new Exception($"Parking session with registration {request.VehicleReg} not found");

            session.TimeOut = DateTime.UtcNow;
            var parkingSpace = await _spaceRepository.GetByIdAsync(session.ParkingSpaceId, cancellationToken) ?? throw new Exception($"Parking space with ID {session.ParkingSpaceId} not found");
            parkingSpace.Status = ParkingSpaceStatus.Available;
            await _spaceRepository.SaveChangesAsync(cancellationToken);
            return new FinishParkingSessionResult(session.Registration, CalculateParkingFee(session.TimeIn, session.TimeOut.Value, session.VehicleType), session.TimeIn, session.TimeOut.Value);
        }

        private static double CalculateParkingFee(DateTime timeIn, DateTime timeOut, VehicleType vehicleType)
        {
            var totalMinutes = Math.Ceiling((timeOut - timeIn).TotalMinutes);
            var feePerMinute = vehicleType switch
            {
                VehicleType.Small => 0.1,
                VehicleType.Medium => 0.2,
                VehicleType.Large => 0.4,
                _ => throw new ArgumentOutOfRangeException(
                 nameof(vehicleType), vehicleType, "Unsupported vehicle type")
            };
            return Math.Round(totalMinutes * feePerMinute + Math.Floor(totalMinutes / 5), 2);
        }
    }
}
