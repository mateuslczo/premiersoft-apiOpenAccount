using BankMore.OpenAccount.Api.Models.Requests;
using BankMore.OpenAccount.Api.Models.Responses;

namespace BankMore.OpenAccount.Api.Interfaces
{
	public interface IAccountCurrentService
	{

		Task<(bool Success, string Message, Guid? AccountId)> AccountRegistrationAsync(AccountOpenRequest request, string token);
		Task<(bool Success, string Message, Guid? AccountId)> DeactivateAccountAsync(AccountDeactivateRequest request, string token);
		

	}
}
