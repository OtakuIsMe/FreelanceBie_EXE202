using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.controllers
{
    [ApiController]
    [Route("api/communication")]
    public class CommunicationController : ControllerBase
    {
        private readonly ICommunicationServ _communicationServ;
        public CommunicationController(ICommunicationServ communicationServ)
        {
            _communicationServ = communicationServ;
        }
        [HttpGet("view-all-communications")]
        public async Task<IActionResult> GetCommunications()
        {
            Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
            return await _communicationServ.GetAllCommunications(userId);
        }
        [HttpGet("view-messages")]
        public async Task<IActionResult> GetMessages(Guid communicationId)
        {
            return await _communicationServ.GetMessages(communicationId);
        }
        [HttpPost("get-in-touch")]
        public async Task<IActionResult> GetInTouch([FromForm] Guid postId, [FromBody] string message)
        {
            Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
            return await _communicationServ.GetInTouch(userId, postId, message);
        }
        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromForm] Guid communicationId, [FromBody] string message)
        {
            Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
            return await _communicationServ.SendMessage(communicationId, userId, message);
        }
    }
}