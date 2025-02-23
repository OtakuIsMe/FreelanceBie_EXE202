using BE.src.api.domains.DTOs.Membership;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.controllers
{
	[ApiController]
	[Route("membership")]
	public class MembershipController : ControllerBase
	{
		private readonly IMembershipServ _membershipServ;
		public MembershipController(IMembershipServ membershipServ)
		{
			_membershipServ = membershipServ;
		}
		[HttpGet("view")]
		public async Task<IActionResult> GetMemberships()
		{
			return await _membershipServ.GetMemberships();
		}
		[HttpPost("create")]
		public async Task<IActionResult> CreateMembership([FromBody] MembershipCreateDTO membership)
		{
			return await _membershipServ.CreateMembership(membership);
		}
		[HttpPut("update/{id}")]
		public async Task<IActionResult> UpdateMembership(Guid id, [FromForm] MembershipUpdateDTO membership)
		{
			return await _membershipServ.UpdateMembership(id, membership);
		}
		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> DeleteMembership(Guid id)
		{
			return await _membershipServ.DeleteMembership(id);
		}
	}
}
