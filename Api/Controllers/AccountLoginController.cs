using BankMore.OpenAccount.Api.ExtensionMethods;
using BankMore.OpenAccount.Api.Interfaces;
using BankMore.OpenAccount.Api.Models.Requests;
using BankMore.OpenAccount.Api.Validators.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.OpenAccount.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AccountLoginController :ControllerBase
	{

		private readonly IAccountLoginService _service;

		public AccountLoginController(IAccountLoginService service)
		{
			_service = service;
		}

		[HttpPost]
		public async Task<IActionResult> Login([FromBody] AccountLoginRequest request)
		{
			try
			{

				var result = await _service.LoginAccountAsync(request);

				if (!result.Success)
					return BadRequest(new { message = result.Message, type = "INVALID_DOCUMENT" });

				var response = new
				{
					success = true,
					message = "Login realizado com sucesso",
					token = result.Token,

				};
				return Ok(response);
			}
			catch (CustomExceptions ex)
			{
				return ex.ToActionResult();
			}

		}
	}
}