using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BE.src.api.domains.DTOs.User
{
	public class UserSearchingDTO
	{
		public string? Name { get; set; }
		public string? Username { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public string? Education { get; set; }
	}
}
