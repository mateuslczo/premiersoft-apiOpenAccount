using BankMore.OpenAccount.Api.Models.Requests;

namespace BankMore.OpenAccount.Api.Interfaces
{
	public interface IAccountRegistrationService
	{
		Task<(bool Success, string Message, Guid? AccountId)> RegisterAsync(OpenAccountRequest request);

	}
}
