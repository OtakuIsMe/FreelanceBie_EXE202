using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.src.api.domains.Enum;

namespace BE.src.api.domains.DTOs.User
{
	public class UserRegisterDTO
	{
		public required string UserName { get; set; }
		public required string Email { get; set; }
		public required string Password { get; set; }
	}
}
