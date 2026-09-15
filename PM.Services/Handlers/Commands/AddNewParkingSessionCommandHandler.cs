using MediatR;
using PM.Core.Entities;
using PM.Core.Entities.Enums;
using PM.DataAccess.Repositories;
using PM.Services.Infra.Commands;
using PM.Services.Infra.Dto;

namespace PM.Services.Handlers.Commands
{
    public class AddNewParkingSessionCommandHandler(IParkingSessionRepository sessionRepository, IParkingSpaceRepository spaceRepository) : IRequestHandler<AddNewParkingSessionCommand, AddNewParkingSessionResult>
    {
        private readonly IParkingSessionRepository _sessionRepository = sessionRepository;
        private readonly IParkingSpaceRepository _spaceRepository = spaceRepository;

        public async Task<AddNewParkingSessionResult> Handle(AddNewParkingSessionCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.VehicleReg)) throw new Exception("VehicleReg cannot be empty");
            if ((await _sessionRepository.FindAsync(x => x.Registration == request.VehicleReg && !x.TimeOut.HasValue, cancellationToken)).Any()) throw new Exception("A active parking session for this vehicle already exists");
            var parkingSpace = (await _spaceRepository.FindAsync(x => x.Status == ParkingSpaceStatus.Available, cancellationToken)).FirstOrDefault() ?? throw new Exception("No available parking space found");
            var session = new ParkingSession
            {
                Id = 0,
                Registration = request.VehicleReg,
                VehicleType = request.VehicleType,
                TimeIn = DateTime.UtcNow,
                ParkingSpaceId = parkingSpace.Id
            };
            parkingSpace.Status = ParkingSpaceStatus.Occupied;
            await _sessionRepository.AddAsync(session, cancellationToken);
            await _spaceRepository.SaveChangesAsync(cancellationToken);
            return new AddNewParkingSessionResult(request.VehicleReg, session.ParkingSpaceId, session.TimeIn);
        }
    }
}
