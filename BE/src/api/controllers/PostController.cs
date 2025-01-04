using BE.src.api.domains.DTOs.Post;
using BE.src.api.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.controllers
{
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
	}
}
