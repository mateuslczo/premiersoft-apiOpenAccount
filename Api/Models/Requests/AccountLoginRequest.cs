namespace BankMore.OpenAccount.Api.Models.Requests
{
	public class AccountLoginRequest
	{
		public int IdContaCorrente { get; set; }
		public string Cpf { get; set; } = "91985494051";
		public string Senha { get; set; } = "12345";

	}
}
