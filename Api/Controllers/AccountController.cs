using BankMore.OpenAccount.Api.ExtensionMethods;
using BankMore.OpenAccount.Api.Interfaces;
using BankMore.OpenAccount.Api.Models.Requests;
using BankMore.OpenAccount.Api.Validators.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.OpenAccount.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AccountController :ControllerBase
	{

		private readonly IAccountCurrentService _service;

		public AccountController(IAccountCurrentService service)
		{
			_service = service;
		}

		[HttpPost("CreateAccountCurrent")]
		public async Task<IActionResult> Register([FromBody] AccountOpenRequest request)
		{
			try
			{

				var result = await _service.AccountRegistrationAsync(request);

				if (!result.Success)
					return BadRequest(new { message = result.Message, type = "INVALID_OPERATION" });

				var response = new
				{
					success = true,
					AccountNumber = result.AccountId,
					message = result.Message,
				};

				return Ok(response);
			}
			catch (CustomExceptions ex)
			{
				return ex.ToActionResult();
			}

		}

		[HttpPost("TransactionAccountCurrent")]
		public async Task<IActionResult> Transaction([FromBody] AccountTransactionRequest request)
		{
			try
			{

				var result = await _service.TransactionAccountAsync(request);

				if (!result.Success)
					return BadRequest(new { message = result.Message, type = "INVALID_OPERATION" });

				var response = new
				{
					success = true,
					AccountNumber = result.AccountId,
					message = result.Message,
				};

				return Ok(response);
			}
			catch (CustomExceptions ex)
			{
				return ex.ToActionResult();
			}

		}

		[HttpPost("DeactivateAccountCurrent")]
		public async Task<IActionResult> DeactivateCurrentAccount([FromBody] AccountDeactivateRequest request)
		{
			try
			{

				var result = await _service.DeactivateAccountAsync(request);

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