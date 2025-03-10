using BE.src.api.domains.DTOs.ElasticSearch;
using BE.src.api.domains.Model;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.controllers
{
    [ApiController]
    [Route("api/v1/elastic-search")]
    public class ElasticSearchController : ControllerBase
    {
        private readonly IElasticSeachServ<User> _designerService;
        private readonly IElasticSeachServ<PostJob> _postService;
        public ElasticSearchController(IElasticSeachServ<User> designerService, IElasticSeachServ<PostJob> postService)
        {
            _designerService = designerService;
            _postService = postService;
        }
        [HttpGet("publish-designers")]
        public async Task<IActionResult> PublishDesigners()
        {
            await _designerService.SyncAllDataAsync();
            return Ok(new { message = "Designers published to ElasticSearch" });
        }

        [HttpGet("publish-posts")]
        public async Task<IActionResult> PublishPosts()
        {
            await _postService.SyncAllDataAsync();
            return Ok(new { message = "Posts published to ElasticSearch" });
        }

        [HttpGet("all-designers")]
        public async Task<IActionResult> GetAllDesigners()
        {
            var designers = await _designerService.GetAllDocumentsAsync();
            return Ok(designers);
        }

        [HttpGet("all-posts")]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _postService.GetAllDocumentsAsync();
            return Ok(posts);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchDesigners([FromQuery] string query)
        {
            var designers = await _designerService.SearchAsync(new List<string> 
            {"name", "username", "email", "city", "education"}, query);
            var posts = await _postService.SearchAsync(new List<string> 
            {"title", "worktype", "worklocation", "companyname", "employmenttype", "experience"}, query);
            return Ok(new { designers, posts });
        }

        [HttpGet("suggest")]
        public async Task<IActionResult> SuggestDesigners([FromQuery] string prefix)
        {
            var designers = await _designerService.SuggestAsync(new List<string> 
            {"name", "username", "email", "city", "education"}, prefix);
            var posts = await _postService.SuggestAsync(new List<string> 
            {"title", "worktype", "worklocation", "companyname", "employmenttype", "experience"}, prefix);
            return Ok(new { designers, posts });
        }
    }
}