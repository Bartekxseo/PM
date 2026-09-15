using MediatR;
using PM.Core.Entities.Enums;
using PM.Services.Infra.Dto;
using System.ComponentModel.DataAnnotations;

namespace PM.Services.Infra.Commands
{
    public record AddNewParkingSessionCommand(string VehicleReg, [property: EnumDataType(typeof(VehicleType))] VehicleType VehicleType) : IRequest<AddNewParkingSessionResult>;
}
