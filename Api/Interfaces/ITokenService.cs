namespace BankMore.OpenAccount.Api.Interfaces
{

	public interface ITokenService
	{
		string GenerateToken(string userId, string userPassword, string role);
	}
}