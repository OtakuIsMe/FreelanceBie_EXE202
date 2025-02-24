using BE.src.api.domains.DTOs.Shot;
using BE.src.api.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.controllers
{
	[ApiController]
	[Route("shot")]
	public class ShotController : ControllerBase
	{
		private readonly IShotServ _shotServ;
		public ShotController(IShotServ shotServ)
		{
			_shotServ = shotServ;
		}
		[Authorize(Policy = "Customer")]
		[HttpPost("AddShotData")]
		public async Task<IActionResult> AddShotData([FromForm] ShotAddData data)
		{
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _shotServ.AddShotData(data, userId);
		}
		[Authorize(Policy = "Customer")]
		[HttpGet("LikeShot")]
		public async Task<IActionResult> LikeShot([FromQuery] Guid shotId, [FromQuery] bool state)
		{
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _shotServ.LikeShot(userId, shotId, state);
		}

		[Authorize(Policy = "Customer")]
		[HttpGet("ShotOwner")]
		public async Task<IActionResult> ShotOwner()
		{
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _shotServ.ShotOwner(userId);
		}

		[AllowAnonymous]
		[HttpGet("ShotDetail")]
		public async Task<IActionResult> Shot([FromQuery] string shotCode)
		{
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
			return await _shotServ.GetShots(filter);
		}
	}
}
