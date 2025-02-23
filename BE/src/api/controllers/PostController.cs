using BE.src.api.domains.DTOs.Post;
using BE.src.api.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.controllers
{
	[ApiController]
	[Route("post")]
	public class PostController : ControllerBase
	{
		private readonly IPostServ _postServ;
		public PostController(IPostServ postServ)
		{
			_postServ = postServ;
		}
		[Authorize(Policy = "Customer")]
		[HttpPost("PostingJob")]
		public async Task<IActionResult> AddPostData([FromForm] PostAddData data)
		{
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _postServ.AddPostData(userId, data);
		}
		[Authorize(Policy = "Customer")]
		[HttpGet("ApplyJob")]
		public async Task<IActionResult> ApplyJob([FromQuery] Guid postId)
		{
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _postServ.ApplyJob(userId, postId);
		}

		[AllowAnonymous]
		[HttpGet("PostJobDetail")]
		public async Task<IActionResult> PostJobDetail([FromQuery] string postCode)
		{
			Guid? userId = null;

			if (User?.Identity?.IsAuthenticated == true)
			{
				userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			}
			return await _postServ.PostJobDetail(userId, postCode);
		}

		// [Authorize(Policy = "Customer")]
		// [HttpGet("HistoryHiring")]
		// public async Task<IActionResult> HistoryHiring([FromQuery] Guid postId)
		// {
		// 	return await _postServ.
		// }
	}
}
