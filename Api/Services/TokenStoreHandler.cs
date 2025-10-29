using BankMore.OpenAccount.Api.Interfaces;
using System.Net.Http.Headers;

namespace BankMore.OpenAccount.Api.Services
{
	/// <summary>
	/// Classe que armazena tokens para requisições em sequencia
	/// </summary>
	public class TokenStoreHandler :DelegatingHandler
	{
		private readonly ITokenService _tokenService;

		public TokenStoreHandler(ITokenService tokenService)
		{
			_tokenService = tokenService;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var token = _tokenService.GetToken();
			if (!string.IsNullOrEmpty(token))
			{
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			}

			return await base.SendAsync(request, cancellationToken);
		}
	}
}