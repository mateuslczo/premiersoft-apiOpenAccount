namespace BankMore.OpenAccount.Api.Models.Responses
{

	public class AccountLoginResponse
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public int? IdContaCorrente { get; set; }
		public string Error { get; set; }
		public string Tipo { get; set; }
        public string Token { get; set; }

    }
}
