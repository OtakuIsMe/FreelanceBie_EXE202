using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BE.src.api.domains.DTOs.Membership
{
    public class MembershipCreateDTO
    {
        public required string Name { get; set; }
		public required float Price { get; set; }
		public required int ExpireTime { get; set; }
		public required string Description { get; set; } = null!;
    }
}