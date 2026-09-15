using PM.Core.Entities.Abstract;
using PM.Core.Entities.Enums;

namespace PM.Core.Entities
{
    public class ParkingSession : Entity<int>
    {
        public string Registration { get; set; } = "";
        public VehicleType VehicleType { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public int ParkingSpaceId { get; set; }
        public virtual ParkingSpace ParkingSpace { get; set; }
    }
}
