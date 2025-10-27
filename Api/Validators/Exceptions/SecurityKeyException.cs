using BankMore.OpenAccount.Api.Validators.Exceptions;

namespace BankMore.OpenAccount.Api.Validators.Exceptions
{

	public class SecurityKeyException :CustomExceptions
	{
		public SecurityKeyException(string message)
			: base("SECURITY_KEY_ERROR", message, 500) { }

		public SecurityKeyException(string message, Exception innerException)
			: base("SECURITY_KEY_ERROR", message, innerException) { }
	}
}