using BE.src.api.shared.Constant;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.shared.Type
{
	public static class ErrorResp
	{
		public static IActionResult BadRequest(string? message)
		{
			return new JsonResult(new { Error = message ?? RespMsg.BAD_REQUEST }) { StatusCode = RespCode.BAD_REQUEST };
		}
		public static IActionResult NotFound(string? message)
		{
			return new JsonResult(new { Error = message ?? RespMsg.NOT_FOUND }) { StatusCode = RespCode.NOT_FOUND };
		}
	}
}
