using BE.src.api.domains.DTOs.Specialty;
using BE.src.api.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.controllers
{
	[ApiController]
	[Route("api/v1/specialty")]
	public class SpecialtyController : ControllerBase
	{
		private readonly ISpecialtyServ _specialtyServ;
		private readonly ILogger<SpecialtyController> _logger;
		public SpecialtyController(ISpecialtyServ specialtyServ, ILogger<SpecialtyController> logger)
		{
			_specialtyServ = specialtyServ;
			_logger = logger;
		}
		[HttpGet("view-specialties")]
		public async Task<IActionResult> ViewSpecialties([FromQuery] string? query)
		{
			_logger.LogInformation("View specialties");
			return await _specialtyServ.ViewSpecialties(query);
		}
		[Authorize(Policy = "Staff")]
		[HttpPost("add-specialty")]
		public async Task<IActionResult> AddSpecialty([FromBody] SpecialtyCreateDTO specialty)
		{
			_logger.LogInformation("Add specialty");
			return await _specialtyServ.AddSpecialty(specialty);
		}
		[Authorize(Policy = "Staff")]
		[HttpPut("update-specialty")]
		public async Task<IActionResult> UpdateSpecialty(Guid id, [FromBody] SpecialtyCreateDTO specialty)
		{
			_logger.LogInformation("Update specialty");
			return await _specialtyServ.UpdateSpecialty(id, specialty);
		}
		[Authorize(Policy = "Staff")]
		[HttpDelete("delete-specialty")]
		public async Task<IActionResult> DeleteSpecialty(Guid id)
		{
			_logger.LogInformation("Delete specialty");
			return await _specialtyServ.DeleteSpecialty(id);
		}
	}
}
