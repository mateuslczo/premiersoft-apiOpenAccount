namespace BankMore.OpenAccount.Api.Models.Responses
{
	public class CreateAccountResponse
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public Guid? IdContaCorrente { get; set; }
		public string Error { get; set; }
		public string Tipo { get; set; }
	}
}