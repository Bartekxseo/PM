using MediatR;
using Microsoft.AspNetCore.Mvc;
using PM.Services.Infra.Commands;
using PM.Services.Infra.Dto;
using PM.Services.Infra.Queries;

namespace PM.Server.Controllers
{
    [Route("api/parking")]
    [ApiController]
    public class ParkingController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost()]
        public async Task<AddNewParkingSessionResult> AddNewParkingSession(AddNewParkingSessionCommand command)
        {
            var result = await _mediator.Send(command);
            return result;
        }

        [HttpGet()]
        public async Task<GetParkingSpaceStatsResult> GetParkingSpaceStats()
        {
            var result = await _mediator.Send(new GetParkingSpaceStatsQuerry());
            return result;
        }

        [HttpPost("exit")]
        public async Task<FinishParkingSessionResult> FinishParkingSession(FinishParkingSessionCommand command)
        {
            var result = await _mediator.Send(command);
            return result;
        }
    }
}
