using BankMore.OpenAccount.Api.Models.Requests;

namespace BankMore.OpenAccount.Api.Interfaces
{
	public interface IAccountCurrentService
	{

		/// <summary>
		/// Cadastrar conta corrente
		/// </summary>
		/// <param name="request"></param>
		/// <returns>Sucesso/Falha</returns>
		/// <exception cref="CustomExceptions"></exception>
		Task<(bool Success, string Message, int? AccountId)> AccountRegistrationAsync(AccountOpenRequest request);
	
		/// <summary>
		/// Desativar uma conta corrente
		/// </summary>
		/// <param name="request"></param>
		/// <returns>Sucesso/Falha</returns>
		/// <exception cref="CustomExceptions"></exception>
		Task<(bool Success, string Message, int? AccountId)> DeactivateAccountAsync(AccountDeactivateRequest request);

		/// <summary>
		/// Moivmentar  conta corrente
		/// </summary>
		/// <param name="request"></param>
		/// <returns>Sucesso/Falha</returns>
		/// <exception cref="CustomExceptions"></exception>
		Task<(bool Success, string Message, int? AccountId)> TransactionAccountAsync(AccountTransactionRequest request);

	}
}
