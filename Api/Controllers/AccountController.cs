using BankMore.OpenAccount.Api.ExtensionMethods;
using BankMore.OpenAccount.Api.Interfaces;
using BankMore.OpenAccount.Api.Models.Requests;
using BankMore.OpenAccount.Api.Validators.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.OpenAccount.API.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]
	public class AccountController :ControllerBase
	{

		private readonly IAccountCurrentService _service;

		public AccountController(IAccountCurrentService service)
		{
			_service = service;
		}

		[HttpPost("CreateAccountCurrent")]
		public async Task<IActionResult> Register([FromBody] AccountOpenRequest request, string token)
		{
			try
			{

				var result = await _service.AccountRegistrationAsync(request, token);

				if (!result.Success)
					return BadRequest(new { message = result.Message, type = "INVALID_DOCUMENT" });

				return Ok(result);
			}
			catch (CustomExceptions ex)
			{
				return ex.ToActionResult();
			}

		}

		[HttpGet("DeactivateAccountCurrent")]
		public async Task<IActionResult> DeactivateCurrentAccount([FromBody] AccountDeactivateRequest request, string token)
		{
			try
			{

				var result = await _service.DeactivateAccountAsync(request, token);

				if (!result.Success)
					return BadRequest(new { message = result.Message, type = "INVALID_ACCOUNT" });

				return Ok(result);
			}
			catch (CustomExceptions ex)
			{
				return ex.ToActionResult();
			}
		}
	}
}