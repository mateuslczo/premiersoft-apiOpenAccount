using BankMore.OpenAccount.Api.Interfaces;
using BankMore.OpenAccount.Api.Models.Requests;
using BankMore.OpenAccount.Api.Models.Responses;
using BankMore.OpenAccount.Api.Services.EndpointsFactory;
using BankMore.OpenAccount.Api.Validators.Exceptions;
using BankMore.OpenAccount.Api.Validators.ValueObjects;
using Microsoft.Extensions.Options;
using System.Text.Json;

public class AccountCurrentService :IAccountCurrentService
{
	private readonly HttpClient _httpClient;
	private readonly ITokenService _tokenService;
	private readonly ILogger<AccountCurrentService> _logger;
	private readonly ApiSettings _apiSettings;

	public AccountCurrentService(
		HttpClient httpClient,
		ITokenService tokenService,
		ILogger<AccountCurrentService> logger, IOptions<ApiSettings> apiOptions)
	{
		_httpClient = httpClient;
		_tokenService = tokenService;
		_logger = logger;
		_apiSettings = apiOptions.Value;
	}

	public async Task<(bool Success, string Message, int? AccountId)> AccountRegistrationAsync(AccountOpenRequest request)
	{
		_logger.LogInformation("Iniciando registro de conta para CPF: {Cpf}", request.Cpf);

		var cpfResult = CpfValidator.Validate(request.Cpf);
		if (cpfResult.IsFailure)
			throw new CustomExceptions("INVALID_DOCUMENT", cpfResult.Error);

		var endpoint = _apiSettings.Endpoints.OpenCurrentAccount;

		var response = await _httpClient.PostAsJsonAsync(endpoint, request);

		if (!response.IsSuccessStatusCode)
		{
			var errorContent = await response.Content.ReadAsStringAsync();
			var errorResponse = JsonSerializer.Deserialize<HttpErrorResponse>(errorContent);
			_logger.LogError("Erro ao criar conta. Status: {StatusCode}, Response: {Error}", response.StatusCode, errorContent);
			throw new CustomExceptions(errorResponse != null ? errorResponse.type : "", "Não foi possivel abrir conta", (int)response.StatusCode);

		}

		var created = await response.Content.ReadFromJsonAsync<AccountOpenResponse>();

		_logger.LogInformation("Conta criada com sucesso. ID: {AccountId}", created?.IdContaCorrente);

		return (true, "Conta criada com sucesso.", created?.IdContaCorrente);
	}

	public async Task<(bool Success, string Message, int? AccountId)> TransactionAccountAsync(AccountTransactionRequest request)
	{
		_logger.LogInformation("Iniciando transação de débito/crédito: {AccountId}", request.IdContaCorrente);

		var endpoint = _apiSettings.Endpoints.Account;
		var response = await _httpClient.GetAsync($"{endpoint}/{request.IdContaCorrente}");

		if (!response.IsSuccessStatusCode)
		{
			var errorContent = await response.Content.ReadAsStringAsync();
			var errorResponse = JsonSerializer.Deserialize<HttpErrorResponse>(errorContent);
			_logger.LogError("Conta invalida ou não encontrada. Status: {StatusCode}, Response: {Error}", response.StatusCode, errorContent);
			throw new CustomExceptions(errorResponse != null ? errorResponse.type : "", "Não foi possivel movimentar a conta", (int)response.StatusCode);
		}

		var account = await response.Content.ReadFromJsonAsync<AccountCurrentResponse>();

		if (!account.ValidarSenha(request.Senha))
		{
			_logger.LogWarning("Senha inválida para a conta {AccountId}", account.IdContaCorrente);
			throw new CustomExceptions("INVALID_CREDENTIALS", "Senha inválida", 403);
		}

		endpoint = _apiSettings.Endpoints.CurrentAccountTransactions;	
		var transactionResponse = await _httpClient.PutAsJsonAsync(endpoint, request);

		if (!transactionResponse.IsSuccessStatusCode)
		{
			var errorContent = await transactionResponse.Content.ReadAsStringAsync();
			var errorResponse = JsonSerializer.Deserialize<HttpErrorResponse>(errorContent);
			_logger.LogError("Erro ao movimentar a conta {AccountId}. Response: {Error}", account.IdContaCorrente, errorContent);
			throw new CustomExceptions(errorResponse != null ? errorResponse.type : "", "Erro ao movimentar conta", (int)transactionResponse.StatusCode);
		}

		_logger.LogInformation("Transação realizada com sucesso {AccountId}", request.IdContaCorrente);
		return (true, "Transação realizada com sucesso", request.IdContaCorrente);
	}


	public async Task<(bool Success, string Message, int? AccountId)> DeactivateAccountAsync(AccountDeactivateRequest request)
	{
		_logger.LogInformation("Iniciando desativação da conta: {AccountId}", request.IdContaCorrente);

		var endpoint = _apiSettings.Endpoints.Account;
		var response = await _httpClient.GetAsync($"{endpoint}/{request.IdContaCorrente}");

		if (!response.IsSuccessStatusCode)
			throw new CustomExceptions("INVALID_ACCOUNT", "Conta corrente não encontrada ou inválida", 400);

		var account = await response.Content.ReadFromJsonAsync<AccountCurrentResponse>();

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

		endpoint = _apiSettings.Endpoints.DeactiveAccount;
		var updateResponse = await _httpClient.PutAsJsonAsync(endpoint, updateRequest);

		if (!updateResponse.IsSuccessStatusCode)
		{
			var errorContent = await updateResponse.Content.ReadAsStringAsync();
			var errorResponse = JsonSerializer.Deserialize<HttpErrorResponse>(errorContent);
			_logger.LogError("Erro ao desativar conta {AccountId}. Response: {Error}", account.IdContaCorrente, errorContent);
			throw new CustomExceptions(errorResponse != null ? errorResponse.type : "", "Erro ao desativar conta", (int)updateResponse.StatusCode);
		}

		_logger.LogInformation("Conta {AccountId} desativada com sucesso", request.IdContaCorrente);
		return (true, "Conta desativada com sucesso", request.IdContaCorrente);
	}

}
