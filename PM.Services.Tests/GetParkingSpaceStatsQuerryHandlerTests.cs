using Moq;
using PM.Core.Entities;
using PM.Core.Entities.Enums;
using PM.DataAccess.Repositories;
using PM.Services.Handlers.Queries;
using PM.Services.Infra.Queries;
using System.Linq.Expressions;

namespace PM.Services.Tests
{
    public class GetParkingSpaceStatsQuerryHandlerTests
    {
        private readonly Mock<IParkingSpaceRepository> _spaceRepository = new(MockBehavior.Strict);

        private GetParkingSpaceStatsQuerryHandler CreateHandler()
            => new(_spaceRepository.Object);

        private readonly List<ParkingSpace> _spaces = [];

        private void SetupReads()
        {
            _spaceRepository.Setup(x => x.GetCountAsync(
                It.IsAny<Expression<Func<ParkingSpace, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Expression<Func<ParkingSpace, bool>> predicate, CancellationToken token) => _spaces.Count(predicate.Compile()));
        }

        [Fact]
        public async Task Handle_WhenAllSpacesAvailable_ReturnsZeroOcupied()
        {
            _spaces.AddRange(
                new ParkingSpace()
                {
                    Id = 1,
                    SpaceNumber = 1,
                    Status = ParkingSpaceStatus.Available
                }, new ParkingSpace()
                {
                    Id = 2,
                    SpaceNumber = 2,
                    Status = ParkingSpaceStatus.Available
                }
            );
            SetupReads();
            var result = await CreateHandler().Handle(new GetParkingSpaceStatsQuerry(), CancellationToken.None);

            Assert.Equal(0, result.OccupiedSpaces);
            Assert.Equal(2, result.AvailableSpaces);
        }

        [Fact]
        public async Task Handle_WhenAllSpacesOcupied_ReturnsZeroAvailable()
        {
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
                    Status = ParkingSpaceStatus.Occupied
                }
            );
            SetupReads();
            var result = await CreateHandler().Handle(new GetParkingSpaceStatsQuerry(), CancellationToken.None);

            Assert.Equal(0, result.AvailableSpaces);
            Assert.Equal(2, result.OccupiedSpaces);
        }

        [Fact]
        public async Task Handle_WhenVaryingSpaceAvailability_ReturnsCorrectCounts()
        {
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
                }
            );
            SetupReads();

            var result = await CreateHandler().Handle(new GetParkingSpaceStatsQuerry(), CancellationToken.None);

            Assert.Equal(1, result.OccupiedSpaces);
            Assert.Equal(1, result.AvailableSpaces);
        }
    }
}
