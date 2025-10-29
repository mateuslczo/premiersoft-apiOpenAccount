namespace BankMore.OpenAccount.Api.Services.EndpointsFactory
{
	public class ApiSettings
	{
		public string BaseUrl { get; set; } = string.Empty;
		public Endpoints Endpoints { get; set; } = new Endpoints();
	}
}
