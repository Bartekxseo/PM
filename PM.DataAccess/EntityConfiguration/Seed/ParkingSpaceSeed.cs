using PM.Core.Entities;

namespace PM.DataAccess.EntityConfiguration.Seed
{
    public static class ParkingSpaceSeed
    {
        public static ParkingSpace[] Entries =>
            [
                new ParkingSpace { Id = 1, SpaceNumber = 1, Status = Core.Entities.Enums.ParkingSpaceStatus.Available },
                new ParkingSpace { Id = 2, SpaceNumber = 2, Status = Core.Entities.Enums.ParkingSpaceStatus.Available },
                new ParkingSpace { Id = 3, SpaceNumber = 3, Status = Core.Entities.Enums.ParkingSpaceStatus.Available },
                new ParkingSpace { Id = 4, SpaceNumber = 4, Status = Core.Entities.Enums.ParkingSpaceStatus.Available },
                new ParkingSpace { Id = 5, SpaceNumber = 5, Status = Core.Entities.Enums.ParkingSpaceStatus.Available },
            ];
    }
}
