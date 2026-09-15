using Moq;
using PM.Core.Entities;
using PM.Core.Entities.Enums;
using PM.DataAccess.Repositories;
using PM.Services.Handlers.Commands;
using PM.Services.Infra.Commands;
using System.Linq.Expressions;

namespace PM.Services.Tests
{
    public class AddNewParkingSessionCommandHandlerTests
    {
        private readonly Mock<IParkingSessionRepository> _sessionRepository = new(MockBehavior.Strict);
        private readonly Mock<IParkingSpaceRepository> _spaceRepository = new(MockBehavior.Strict);

        private AddNewParkingSessionCommandHandler CreateHandler()
            => new(_sessionRepository.Object, _spaceRepository.Object);

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
                .Setup(r => r.FindAsync(
                    It.IsAny<Expression<Func<ParkingSpace, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Expression<Func<ParkingSpace, bool>> p, CancellationToken _) =>
                    _spaces.Where(p.Compile()).ToList());
        }

        private void SetupWrites()
        {
            _sessionRepository
                .Setup(r => r.AddAsync(It.IsAny<ParkingSession>(), It.IsAny<CancellationToken>()))
                .Callback<ParkingSession, CancellationToken>((s, _) => _sessions.Add(s))
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
        public async Task Handle_WhenSpaceIsAvailable_CreatesSessionAndOccupiesSpace()
        {
            //Arrange
            _spaces.Add(new ParkingSpace
            {
                Id = 1,
                SpaceNumber = 1,
                Status = ParkingSpaceStatus.Available
            });
            SetupReads();
            SetupWrites();
            var command = new AddNewParkingSessionCommand("ABC123", VehicleType.Medium);

            //Act
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            //Assert
            var session = _sessions.SingleOrDefault();
            var space = _spaces.SingleOrDefault();
            Assert.NotNull(session);
            Assert.Equal(command.VehicleReg, session.Registration);
            Assert.Equal(command.VehicleType, session.VehicleType);
            Assert.Null(session.TimeOut);
            Assert.Equal(space.Id, session.ParkingSpaceId);

            Assert.Equal(command.VehicleReg, result.VehicleReg);
            Assert.Equal(space.SpaceNumber, result.ParkingSpaceNumber);
            Assert.InRange(result.StartTime, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));

            Assert.Equal(ParkingSpaceStatus.Occupied, space.Status);
        }

        [Fact]
        public async Task Handle_WhenSpaceIsOcupied_ThrowsError()
        {
            //Arrange
            SetupReads();
            var command = new AddNewParkingSessionCommand("ABC123", VehicleType.Medium);

            //Act
            var ex = await Assert.ThrowsAsync<Exception>(() => CreateHandler().Handle(command, CancellationToken.None));


            //Assert
            Assert.Equal("No available parking space found", ex.Message);
            _sessionRepository.Verify(x => x.AddAsync(It.IsAny<ParkingSession>(), It.IsAny<CancellationToken>()), Times.Never);
            _spaceRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        }

        [Fact]
        public async Task Handle_WhenBehicleRegIsNullOrEmpty_ThrowsError()
        {
            //Arrange
            var command = new AddNewParkingSessionCommand("", VehicleType.Medium);

            //Act
            var ex = await Assert.ThrowsAsync<Exception>(() => CreateHandler().Handle(command, CancellationToken.None));

            //Assert
            Assert.Equal("VehicleReg cannot be empty", ex.Message);
            _sessionRepository.Verify(x => x.AddAsync(It.IsAny<ParkingSession>(), It.IsAny<CancellationToken>()), Times.Never);
            _spaceRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        }

        [Fact]
        public async Task Handle_WhenSpaceIsAvailableWithTwoSpaces_CreatesSessionAndOccupiesAvailableSpace()
        {
            //Arrange
            _spaces.AddRange(
                new ParkingSpace()
                {
                    Id = 1,
                    SpaceNumber = 1,
                    Status = ParkingSpaceStatus.Occupied
                }, new ParkingSpace()
                {
                    Id = 2,
                    SpaceNumber = 2,
                    Status = ParkingSpaceStatus.Available
                });

            SetupReads();
            SetupWrites();
            var command = new AddNewParkingSessionCommand("ABC123", VehicleType.Medium);

            //Act
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            //Assert
            var session = _sessions.SingleOrDefault();
            Assert.NotNull(session);
            Assert.Equal(command.VehicleReg, session.Registration);
            Assert.Equal(command.VehicleType, session.VehicleType);
            Assert.Null(session.TimeOut);
            Assert.Equal(2, session.ParkingSpaceId);

            Assert.Equal(command.VehicleReg, result.VehicleReg);
            Assert.Equal(2, result.ParkingSpaceNumber);
            Assert.InRange(result.StartTime, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));

            Assert.All(_spaces, s => Assert.Equal(ParkingSpaceStatus.Occupied, s.Status));
        }

        [Fact]
        public async Task Handle_WhenSpaceIsAvailableWithThreeSpaces_CreatesSessionAndOccupiesFirstAvailableSpace()
        {
            //Arrange
            _spaces.AddRange(
                new ParkingSpace()
                {
                    Id = 1,
                    SpaceNumber = 1,
                    Status = ParkingSpaceStatus.Occupied
                }, new ParkingSpace()
                {
                    Id = 2,
                    SpaceNumber = 2,
                    Status = ParkingSpaceStatus.Available
                },
                new ParkingSpace()
                {
                    Id = 3,
                    SpaceNumber = 3,
                    Status = ParkingSpaceStatus.Available
                }
            );

            SetupReads();
            SetupWrites();

            var command = new AddNewParkingSessionCommand("ABC123", VehicleType.Medium);

            //Act
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            //Assert
            var session = _sessions.SingleOrDefault();
            Assert.NotNull(session);
            Assert.Equal(command.VehicleReg, session.Registration);
            Assert.Equal(command.VehicleType, session.VehicleType);
            Assert.Null(session.TimeOut);
            Assert.Equal(2, session.ParkingSpaceId);

            Assert.Equal(command.VehicleReg, result.VehicleReg);
            Assert.Equal(2, result.ParkingSpaceNumber);
            Assert.InRange(result.StartTime, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));

            Assert.Equal(ParkingSpaceStatus.Occupied, _spaces[1].Status);
            Assert.Equal(ParkingSpaceStatus.Available, _spaces[2].Status);
        }
    }
}
