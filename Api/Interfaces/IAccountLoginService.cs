using BankMore.OpenAccount.Api.Models.Requests;

namespace BankMore.OpenAccount.Api.Interfaces
{
	public interface IAccountLoginService
	{
		Task<(bool Success, string Message, Guid? AccountId, string Token)> LoginAccountAsync(AccountLoginRequest request);

	}
}
