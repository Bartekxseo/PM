using Moq;
using PM.Core.Entities;
using PM.Core.Entities.Enums;
using PM.DataAccess.Repositories;
using PM.Services.Handlers.Commands;
using PM.Services.Infra.Commands;
using System.Linq.Expressions;

namespace PM.Services.Tests
{
    public class FinishParkingSessionCommandHandlerTests
    {
        private readonly Mock<IParkingSessionRepository> _sessionRepository = new(MockBehavior.Strict);
        private readonly Mock<IParkingSpaceRepository> _spaceRepository = new(MockBehavior.Strict);

        private FinishParkingSessionCommandHandler CreateHandler()
            => new(_spaceRepository.Object, _sessionRepository.Object);


        private readonly List<ParkingSession> _sessions = [];
        private readonly List<ParkingSpace> _spaces = [];

        private void SetupReads()
        {
            _sessionRepository
                .Setup(r => r.FindAsync(
                    It.IsAny<Expression<Func<ParkingSession, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Expression<Func<ParkingSession, bool>> p, CancellationToken _) =>
                    _sessions.Where(p.Compile()).ToList());

            _spaceRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((int id, CancellationToken _) =>
                    _spaces.SingleOrDefault(s => s.Id == id)!);
        }

        private void SetupWrites()
        {
            _sessionRepository
                .Setup(r => r.AddAsync(It.IsAny<ParkingSession>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _sessionRepository
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _spaceRepository
                .Setup(r => r.AddAsync(It.IsAny<ParkingSpace>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _spaceRepository
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        [Fact]
        public async Task Handle_WhenActiveSessionForMediumVehicleExists_CloseAndFreeSpace()
        {
            //Arrange
            var timeIn = DateTime.UtcNow.AddMinutes(-30);
            _sessions.Add(new ParkingSession
            {
                Id = 1,
                Registration = "ABC123",
                VehicleType = VehicleType.Medium,
                TimeIn = timeIn,
                TimeOut = null,
                ParkingSpaceId = 1
            });
            _spaces.Add(new ParkingSpace
            {
                Id = 1,
                SpaceNumber = 1,
                Status = ParkingSpaceStatus.Occupied
            });
            SetupReads();
            SetupWrites();

            var command = new FinishParkingSessionCommand("ABC123");

            //Act
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            //Assert
            Assert.Equal("ABC123", result.VehicleReg);
            Assert.Equal(timeIn, result.TimeIn);
            Assert.InRange(result.TimeOut, DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));


            Assert.Equal(12.2, result.VehicleCharge, 2);

            Assert.NotNull(_sessions[0].TimeOut);
            Assert.Equal(ParkingSpaceStatus.Available, _spaces.Single(s => s.Id == 1).Status);

            _spaceRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task Handle_WhenActiveSessionForSmallVehicleExists_CloseAndFreeSpace()
        {
            //Arrange
            var timeIn = DateTime.UtcNow.AddMinutes(-30);
            _sessions.Add(new ParkingSession
            {
                Id = 1,
                Registration = "ABC123",
                VehicleType = VehicleType.Small,
                TimeIn = timeIn,
                TimeOut = null,
                ParkingSpaceId = 1
            });
            _spaces.Add(new ParkingSpace
            {
                Id = 1,
                SpaceNumber = 1,
                Status = ParkingSpaceStatus.Occupied
            });
            SetupReads();
            SetupWrites();

            var command = new FinishParkingSessionCommand("ABC123");

            //Act
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            //Assert
            Assert.Equal("ABC123", result.VehicleReg);
            Assert.Equal(timeIn, result.TimeIn);
            Assert.InRange(result.TimeOut, DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));


            Assert.Equal(9.1, result.VehicleCharge, 2);

            Assert.NotNull(_sessions[0].TimeOut);
            Assert.Equal(ParkingSpaceStatus.Available, _spaces.Single(s => s.Id == 1).Status);

            _spaceRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task Handle_WhenActiveSessionForLargeVehicleExists_CloseAndFreeSpace()
        {
            //Arrange
            var timeIn = DateTime.UtcNow.AddMinutes(-30);
            _sessions.Add(new ParkingSession
            {
                Id = 1,
                Registration = "ABC123",
                VehicleType = VehicleType.Large,
                TimeIn = timeIn,
                TimeOut = null,
                ParkingSpaceId = 1
            });
            _spaces.Add(new ParkingSpace
            {
                Id = 1,
                SpaceNumber = 1,
                Status = ParkingSpaceStatus.Occupied
            });
            SetupReads();
            SetupWrites();

            var command = new FinishParkingSessionCommand("ABC123");

            //Act
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            //Assert
            Assert.Equal("ABC123", result.VehicleReg);
            Assert.Equal(timeIn, result.TimeIn);
            Assert.InRange(result.TimeOut, DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));


            Assert.Equal(18.4, result.VehicleCharge, 2);

            Assert.NotNull(_sessions[0].TimeOut);
            Assert.Equal(ParkingSpaceStatus.Available, _spaces.Single(s => s.Id == 1).Status);

            _spaceRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNoSessionExists_ThrowsException()
        {
            //Arrange
            SetupReads();

            var command = new FinishParkingSessionCommand("ABC123");

            //Act
            var ex = await Assert.ThrowsAsync<Exception>(() => CreateHandler().Handle(command, CancellationToken.None));

            //Assert
            Assert.Equal("Parking session with registration ABC123 not found", ex.Message);
        }
    }
}
