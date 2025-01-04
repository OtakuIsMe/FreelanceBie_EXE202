using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BE.src.api.domains.DTOs.User
{
    public class UserChangePwdDTO
    {
        public required string Email { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}