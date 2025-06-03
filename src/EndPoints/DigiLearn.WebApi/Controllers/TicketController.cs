using DigiLearn.WebApi.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketModule.Core.DTOs.Tickets;
using TicketModule.Core.Services;

namespace DigiLearn.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ApiController
    {
        private readonly ITicketService _service;

        public TicketController(ITicketService service)
        {
            _service = service;
        }

        [HttpPost("CreateTicket")]
        public async Task<ApiResult<Guid>> CreateTicket(CreateTicketCommand command)
        {
            var result = await _service.CreateTicket(command);
            return CommandResult<Guid>(result);
        }
        [HttpPost("SendTicketMessage")]
        public async Task<ApiResult> SendMessageInTicket(SendTicketMessageCommand command)
        {
            var result = await _service.SendMessageInTicket(command);
            return CommandResult(result);
        }
        [HttpPost("CloseTicket")]
        public async Task<ApiResult> CloseTicket(Guid ticketId)
        {
            var result = await _service.CloseTicket(ticketId);
            return CommandResult(result);
        }
        [HttpGet("GetTicket")]
        public async Task<ApiResult<TicketDto?>> GetTicket(Guid ticketId)
        {
            var result = await _service.GetTicket(ticketId);
            return QueryResult(result);
        }
        [HttpGet("GetTicketByFilter")]
        public async Task<ApiResult<TicketFilterResult>> GetTicketsByFilter
            ([FromQuery] TicketFilterParams filterParams)
        {
            var result = await _service.GetTicketsByFilter(filterParams);
            return QueryResult(result);
        }
    }
}
