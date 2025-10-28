using BankMore.OpenAccount.Api.Interfaces;
using BankMore.OpenAccount.Api.Models.Requests;
using BankMore.OpenAccount.Api.Models.Responses;
using BankMore.OpenAccount.Api.Validators.Exceptions;
using BankMore.OpenAccount.Api.Validators.ValueObjects;

public class AccountCurrentService :IAccountCurrentService
{
	private readonly HttpClient _httpClient;
	private readonly ITokenService _tokenService;
	private readonly ILogger<AccountCurrentService> _logger;

	public AccountCurrentService(
		HttpClient httpClient,
		ITokenService tokenService,
		ILogger<AccountCurrentService> logger)
	{
		_httpClient = httpClient;
		_tokenService = tokenService;
		_logger = logger;
	}

	public async Task<(bool Success, string Message, Guid? AccountId)> AccountRegistrationAsync(AccountOpenRequest request, string token)
	{
		_logger.LogInformation("Iniciando registro de conta para CPF: {Cpf}", request.Cpf);

		if (!_tokenService.ValidateToken(token))
			throw new CustomExceptions("INVALID_TOKEN", "Token inválido");

		var cpfResult = CpfValidator.Validate(request.Cpf);
		if (cpfResult.IsFailure)
			throw new CustomExceptions("INVALID_DOCUMENT", cpfResult.Error);

		_httpClient.DefaultRequestHeaders.Authorization =
			new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

		var response = await _httpClient.PostAsJsonAsync("api/accounts", request);

		if (!response.IsSuccessStatusCode)
		{
			var errorContent = response.Content.ReadAsStringAsync();
			_logger.LogError("Erro ao criar conta. Status: {StatusCode}, Response: {Error}", response.StatusCode, errorContent);
			throw new CustomExceptions("USER_UNAUTHORIZED", "Não autorizado", (int)response.StatusCode);

		}

		var created = await response.Content.ReadFromJsonAsync<AccountOpenResponse>();

		_logger.LogInformation("Conta criada com sucesso. ID: {AccountId}", created?.IdContaCorrente);

		return (true, "Conta criada com sucesso.", created?.IdContaCorrente);
	}


	/// <summary>
	/// Desativar uma conta corrente
	/// </summary>
	/// <param name="request"></param>
	/// <param name="token"></param>
	/// <returns></returns>
	/// <exception cref="CustomExceptions"></exception>
	public async Task<(bool Success, string Message, Guid? AccountId)> DeactivateAccountAsync(AccountDeactivateRequest request, string token)
	{
		_logger.LogInformation("Iniciando desativação da conta: {AccountId}", request.IdContaCorrente);


		if (!_tokenService.ValidateToken(token))
		{
			_logger.LogWarning("Token inválido ou expirado para a conta {AccountId}", request.IdContaCorrente);
			throw new CustomExceptions("USER_UNAUTHORIZED", "Token inválido ou expirado", 403);
		}

	
		var response = await _httpClient.GetAsync($"api/accounts/{request.IdContaCorrente}");

		if (!response.IsSuccessStatusCode)
			throw new CustomExceptions("INVALID_ACCOUNT", "Conta corrente não encontrada ou inválida", 400);

		var account = await response.Content.ReadFromJsonAsync<AccountCurrentResponse>();

		/// Talvez seja desnecessária essa validação
		if (!account.ValidarSenha(account.Senha))
		{
			_logger.LogWarning("Senha inválida para a conta {AccountId}", account.IdContaCorrente);
			throw new CustomExceptions("INVALID_CREDENTIALS", "Senha inválida", 403);
		}

		// (desativar conta)
		var updateRequest = new
		{
			IdContaCorrente = account.IdContaCorrente,
			Ativo = false
		};

		_httpClient.DefaultRequestHeaders.Authorization =
			new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

		//criar endpoint na APIMED
		var updateResponse = await _httpClient.PutAsJsonAsync("api/accounts/deactivate", updateRequest);

		if (!updateResponse.IsSuccessStatusCode)
		{
			var error = await updateResponse.Content.ReadAsStringAsync();
			_logger.LogError("Erro ao desativar conta {AccountId}. Response: {Error}", account.IdContaCorrente, error);
			throw new CustomExceptions("UPDATE_FAILED", "Erro ao desativar conta", (int)updateResponse.StatusCode);
		}

		_logger.LogInformation("Conta {AccountId} desativada com sucesso", request.IdContaCorrente);
		return (true, "Conta desativada com sucesso", request.IdContaCorrente);
	}

}
