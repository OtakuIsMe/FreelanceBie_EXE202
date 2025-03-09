namespace BE.src.api.domains.DTOs.Transaction
{
	public class CassoDto
	{
		public int Error { get; set; }
		public string Message { get; set; }
		public CassoDataDto Data { get; set; }
	}

	public class CassoDataDto
	{
		public List<CassoRecordDto> Records { get; set; }
	}

	public class CassoRecordDto
	{
		public string Description { get; set; }
		public int Amount { get; set; }
	}
}
