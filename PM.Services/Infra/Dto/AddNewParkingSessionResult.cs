namespace PM.Services.Infra.Dto
{
    public record AddNewParkingSessionResult(
        string VehicleReg,
        int ParkingSpaceNumber,
        DateTime StartTime
    );

}
