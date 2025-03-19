using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BE.src.api.domains.DTOs.Shot
{
	public class ShotSearchFilterDTO
	{
		public string? UserName { get; set; }
		public string? UserEmail { get; set; }
		public string? UserEducation { get; set; }

		public string? SpecialtyName { get; set; }

		public string? HtmlKeyword { get; set; }
		public string? CssKeyword { get; set; }
		public int? MinViews { get; set; }
		public int? MaxViews { get; set; }
	}
}
