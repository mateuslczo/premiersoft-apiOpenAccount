namespace BankMore.OpenAccount.Api.Models.Responses
{
	public class HttpErrorResponse
	{
		public string? error { get; set; }
		public string type { get; set; } = "";
		public int code { get; set; }
	}
}