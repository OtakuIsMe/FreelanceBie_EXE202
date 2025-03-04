using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.src.api.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.controllers
{
    [ApiController]
    [Route("api/v1/communication")]
    public class CommunicationController : ControllerBase
    {
        private readonly ICommunicationServ _communicationServ;
        private readonly ILogger<CommunicationController> _logger;
        public CommunicationController(ICommunicationServ communicationServ, ILogger<CommunicationController> logger)
        {
            _communicationServ = communicationServ;
            _logger = logger;
        }
        [Authorize(Policy = "Customer")]
        [HttpGet("view-all-communications")]
        public async Task<IActionResult> GetCommunications()
        {
            _logger.LogInformation("Getting all communications");
            Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
            return await _communicationServ.GetAllCommunications(userId);
        }
        [Authorize(Policy = "Customer")]
        [HttpGet("view-messages")]
        public async Task<IActionResult> GetMessages(Guid communicationId)
        {
            _logger.LogInformation("Getting all messages");
            return await _communicationServ.GetMessages(communicationId);
        }
        [Authorize(Policy = "Customer")]
        [HttpPost("get-in-touch")]
        public async Task<IActionResult> GetInTouch([FromForm] Guid postId, [FromBody] string message)
        {
            _logger.LogInformation("Getting in touch");
            Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
            return await _communicationServ.GetInTouch(userId, postId, message);
        }
        [Authorize(Policy = "Customer")]
        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromForm] Guid communicationId, [FromBody] string message)
        {
            _logger.LogInformation("Sending message"); 
            Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
            return await _communicationServ.SendMessage(communicationId, userId, message);
        }
    }
}