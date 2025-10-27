using BankMore.OpenAccount.Api.Validators.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.OpenAccount.Api.ExtensionMethods
{

	public static class CustomExceptionExtensions
	{
		public static IActionResult ToActionResult(this CustomExceptions ex)
		{
			var errorResponse = new
			{
				error = ex.Message,
				type = ex.ErrorCode,
				code = ex.HttpStatusCode
			};

			return ex.HttpStatusCode switch
			{
				400 => new BadRequestObjectResult(errorResponse),
				401 => new UnauthorizedObjectResult(errorResponse),
				403 => new ObjectResult(errorResponse) { StatusCode = 403 },
				404 => new NotFoundObjectResult(errorResponse),
				500 => new ObjectResult(errorResponse) { StatusCode = 500 },
				_ => new ObjectResult(errorResponse) { StatusCode = ex.HttpStatusCode }
			};
		}
	}
}