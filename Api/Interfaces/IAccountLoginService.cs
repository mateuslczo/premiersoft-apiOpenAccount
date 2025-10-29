using BankMore.OpenAccount.Api.Models.Requests;

namespace BankMore.OpenAccount.Api.Interfaces
{
	public interface IAccountLoginService
	{
		Task<(bool Success, string Message, int? AccountId, string Token)> LoginAccountAsync(AccountLoginRequest request);

	}
}
