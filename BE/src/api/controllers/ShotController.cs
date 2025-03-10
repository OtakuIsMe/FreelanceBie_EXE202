using BE.src.api.domains.DTOs.Shot;
using BE.src.api.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.controllers
{
	[ApiController]
	[Route("api/v1/shot")]
	public class ShotController : ControllerBase
	{
		private readonly IShotServ _shotServ;
		private readonly ILogger<ShotController> _logger;
		public ShotController(IShotServ shotServ, ILogger<ShotController> logger)
		{
			_shotServ = shotServ;
			_logger = logger;
		}
		[Authorize(Policy = "Customer")]
		[HttpPost("AddShotData")]
		public async Task<IActionResult> AddShotData([FromForm] ShotAddData data)
		{
			_logger.LogInformation("AddShotData");
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _shotServ.AddShotData(data, userId);
		}
		[Authorize(Policy = "Customer")]
		[HttpGet("LikeShot")]
		public async Task<IActionResult> LikeShot([FromQuery] Guid shotId, [FromQuery] bool state)
		{
			_logger.LogInformation("LikeShot");
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _shotServ.LikeShot(userId, shotId, state);
		}

		[Authorize(Policy = "Customer")]
		[HttpGet("ShotOwner")]
		public async Task<IActionResult> ShotOwner()
		{
			_logger.LogInformation("ShotOwner");
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _shotServ.ShotOwner(userId);
		}

		[AllowAnonymous]
		[HttpGet("ShotDetail")]
		public async Task<IActionResult> Shot([FromQuery] Guid shotCode)
		{
			_logger.LogInformation("ShotDetail");
			Guid? userId = null;

			if (User?.Identity?.IsAuthenticated == true)
			{
				userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			}

			return await _shotServ.GetShotDetail(userId, shotCode);
		}
		[HttpGet("search-filter-shots")]
		public async Task<IActionResult> GetShots([FromQuery] ShotSearchFilterDTO filter)
		{
			_logger.LogInformation("GetShots");
			return await _shotServ.GetShots(filter);
		}

		[HttpGet("other-shots")]
		public async Task<IActionResult> OtherShots([FromQuery] Guid ShotId)
		{
			return await _shotServ.OtherShots(ShotId);
		}

		[HttpGet("shot-random")]
		public async Task<IActionResult> ShotRandom([FromQuery] int item)
		{
			return await _shotServ.ShotRandom(item);
		}
	}
}
