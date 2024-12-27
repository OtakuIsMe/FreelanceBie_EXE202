using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BE.src.api.domains.DTOs.Membership
{
    public class MembershipUpdateDTO
    {
        public string? Name { get; set; }
		public float? Price { get; set; }
		public int? ExpireTime { get; set; }
		public string? Description { get; set; }
    }
}