namespace BankMore.OpenAccount.Api.Models.Requests
{
	public class AccountLoginRequest
	{
		public Guid IdContaCorrente { get; set; }
		public string Cpf { get; set; }
		public string Senha { get; set; }

	}
}
