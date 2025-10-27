using BankMore.OpenAccount.Api.Validators.Exceptions;

namespace BankMore.OpenAccount.Api.Validators.Exceptions
{

	public class JwtConfigurationException :CustomExceptions
	{
		public JwtConfigurationException(string message)
			: base("JWT_CONFIGURATION_ERROR", message, 500) { }

		public JwtConfigurationException(string message, Exception innerException)
			: base("JWT_CONFIGURATION_ERROR", message, innerException) { }
	}
}