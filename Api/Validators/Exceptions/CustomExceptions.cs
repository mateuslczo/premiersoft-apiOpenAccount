namespace BankMore.OpenAccount.Api.Validators.Exceptions
{
	public class CustomExceptions :Exception
	{
		public string ErrorCode { get; }
		public int HttpStatusCode { get; }

		public CustomExceptions(string errorCode, string message)
			: base(message)
		{
			ErrorCode = errorCode;
			HttpStatusCode = GetStatusCodeFromErrorCode(errorCode);
		}

		public CustomExceptions(string errorCode, string message, int httpStatusCode)
			: base(message)
		{
			ErrorCode = errorCode;
			HttpStatusCode = httpStatusCode;
		}

		public CustomExceptions(string errorCode, string message, Exception innerException)
	: base(message, innerException)
		{
			ErrorCode = errorCode;
			HttpStatusCode = GetStatusCodeFromErrorCode(errorCode);
		}

		private static int GetStatusCodeFromErrorCode(string errorCode)
		{
			return errorCode switch
			{
				"INVALID_DOCUMENT" => 400,
				"INVALID_TOKEN" => 401,
				"USER_UNAUTHORIZED" => 403,
				"ACCOUNT_NOT_FOUND" => 404,
				"SECURITY_KEY_ERROR" => 500, 
				"JWT_CONFIGURATION_ERROR" => 500, 
				_ => 400
			};
		}
	}

}
