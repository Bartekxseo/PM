using PM.Core.Entities.Abstract;
using PM.Core.Entities.Enums;

namespace PM.Core.Entities
{
    public class ParkingSpace : Entity<int>
    {
        public int SpaceNumber { get; set; }
        public ParkingSpaceStatus Status { get; set; } = ParkingSpaceStatus.Available;
    }
}
