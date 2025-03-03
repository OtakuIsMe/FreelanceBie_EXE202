using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BE.src.api.domains.DTOs.User
{
	public class UserEditProfileDTO
	{
		public string? Name { get; set; }
		public string? Phone { get; set; }
		public string? City { get; set; }
		public string? Education { get; set; }
		public string? Description { get; set; }
		public string? DOB { get; set; }
		public IFormFile? Image { get; set; }
	}
}
