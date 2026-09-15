namespace PM.Services.Infra.Dto
{
    public record FinishParkingSessionResult(
        string VehicleReg,
        double VehicleCharge,
        DateTime TimeIn,
        DateTime TimeOut
        );
}
