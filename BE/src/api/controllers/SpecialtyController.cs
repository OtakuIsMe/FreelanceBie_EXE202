using BE.src.api.domains.DTOs.Specialty;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.controllers
{
	[ApiController]
	[Route("specialty")]
	public class SpecialtyController : ControllerBase
	{
		private readonly ISpecialtyServ _specialtyServ;
		public SpecialtyController(ISpecialtyServ specialtyServ)
		{
			_specialtyServ = specialtyServ;
		}
		[HttpGet("view-specialties")]
		public async Task<IActionResult> ViewSpecialties([FromQuery] string? query)
		{
			return await _specialtyServ.ViewSpecialties(query);
		}
		[HttpPost("add-specialty")]
		public async Task<IActionResult> AddSpecialty([FromBody] SpecialtyCreateDTO specialty)
		{
			return await _specialtyServ.AddSpecialty(specialty);
		}
		[HttpPut("update-specialty")]
		public async Task<IActionResult> UpdateSpecialty(Guid id, [FromBody] SpecialtyCreateDTO specialty)
		{
			return await _specialtyServ.UpdateSpecialty(id, specialty);
		}
		[HttpDelete("delete-specialty")]
		public async Task<IActionResult> DeleteSpecialty(Guid id)
		{
			return await _specialtyServ.DeleteSpecialty(id);
		}
	}
}
