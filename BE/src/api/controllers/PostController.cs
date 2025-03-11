using BE.src.api.domains.DTOs.Post;
using BE.src.api.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.controllers
{
	[ApiController]
	[Route("api/v1/post")]
	public class PostController : ControllerBase
	{
		private readonly IPostServ _postServ;
		private readonly ILogger<PostController> _logger;
		public PostController(IPostServ postServ, ILogger<PostController> logger)
		{
			_postServ = postServ;
			_logger = logger;
		}
		[Authorize(Policy = "Customer")]
		[HttpPost("PostingJob")]
		public async Task<IActionResult> AddPostData([FromForm] PostAddData data)
		{
			_logger.LogInformation("AddPostData");
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _postServ.AddPostData(userId, data);
		}
		[Authorize(Policy = "Customer")]
		[HttpGet("ApplyJob")]
		public async Task<IActionResult> ApplyJob([FromQuery] Guid postId)
		{
			_logger.LogInformation("ApplyJob");
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _postServ.ApplyJob(userId, postId);
		}

		[AllowAnonymous]
		[HttpGet("PostJobDetail")]
		public async Task<IActionResult> PostJobDetail([FromQuery] Guid postCode)
		{
			_logger.LogInformation("PostJobDetail");
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
		[HttpGet("filter-posts")]
		public async Task<IActionResult> GetPosts([FromQuery] PostJobFilterDTO filter)
		{
			_logger.LogInformation("GetPosts");
			return await _postServ.GetPosts(filter);
		}
		[HttpGet("download-attachment")]
		public async Task<IActionResult> GetFile([FromQuery] Guid id)
		{
			var fileResult = await _postServ.DownloadFile(id);
			if (fileResult == null)
			{
				return NotFound("Cannot find attachment");
			}

			return fileResult;
		}
		[HttpGet("list-post-card")]
		public async Task<IActionResult> GetListPostCard([FromQuery] int item, [FromQuery] int page)
		{
			return await _postServ.GetListPostCard(item, page);
		}
	}
}
