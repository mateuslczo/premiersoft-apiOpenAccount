using BankMore.OpenAccount.Api.ExtensionMethods;
using BankMore.OpenAccount.Api.Interfaces;
using BankMore.OpenAccount.Api.Models.Requests;
using BankMore.OpenAccount.Api.Validators.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.OpenAccount.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AccountRegistrationController :ControllerBase
	{

		private readonly IAccountRegistrationService _service;

		public AccountRegistrationController(IAccountRegistrationService service)
		{
			_service = service;
		}

		[HttpPost]
		public async Task<IActionResult> Register([FromBody] OpenAccountRequest request)
		{
			try
			{

				var result = await _service.RegisterAsync(request);

				if (!result.Success)
					return BadRequest(new { message = result.Message, type = "INVALID_DOCUMENT" });

				return Ok(result);
			}
			catch (CustomExceptions ex)
			{
				return ex.ToActionResult(); 
			}

		}
	}
}