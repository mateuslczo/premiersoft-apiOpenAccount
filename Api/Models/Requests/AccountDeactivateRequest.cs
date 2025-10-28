namespace BankMore.OpenAccount.Api.Models.Requests
{
	/// <summary>
	/// Request para desativação da conta corrente
	/// </summary>
	public class AccountDeactivateRequest
	{
		public Guid IdContaCorrente { get; set; }
		public string Cpf { get; set; }
		public string Senha { get; set; }
	}
}
