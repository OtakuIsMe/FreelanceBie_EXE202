namespace BE.src.api.domains.DTOs.Pagination
{
	public class PaginationRp
	{
		public int? TotalPage { get; set; }
		public int? Index { get; set; }
	}
	public class PaginationRq
	{
		public int? CountItem { get; set; }
		public int? Index { get; set; }
	}
}
