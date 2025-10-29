using BankMore.OpenAccount.Api.Interfaces;
using BankMore.OpenAccount.Api.Models.Requests;
using BankMore.OpenAccount.Api.Models.Responses;
using BankMore.OpenAccount.Api.Services.EndpointsFactory;
using BankMore.OpenAccount.Api.Validators.Exceptions;
using BankMore.OpenAccount.Api.Validators.ValueObjects;
using Microsoft.Extensions.Options;
using System.Text.Json;

public class AccountLoginService :IAccountLoginService
{
	private readonly HttpClient _httpClient;
	private readonly ITokenService _tokenService;
	private readonly ILogger<AccountLoginService> _logger;
	private readonly ApiSettings _apiSettings;

	public AccountLoginService(
		HttpClient httpClient,
		ITokenService tokenService,
		ILogger<AccountLoginService> logger, IOptions<ApiSettings> apiOptions)
	{
		_httpClient = httpClient;
		_tokenService = tokenService;
		_logger = logger;
		_apiSettings = apiOptions.Value;
	}

	public async Task<(bool Success, string Message, int? AccountId, string Token)> LoginAccountAsync(AccountLoginRequest request)
	{
		_logger.LogInformation("Iniciando login para: {Cpf} ou ID: {Id}", request.Cpf, request.IdContaCorrente);

		if (string.IsNullOrWhiteSpace(request.Cpf) && request.IdContaCorrente == 0)
			throw new CustomExceptions("INVALID_CREDENTIALS", "CPF ou ID da conta é obrigatório");

		if (!string.IsNullOrWhiteSpace(request.Cpf) && CpfValidator.Validate(request.Cpf).IsFailure)
			throw new CustomExceptions("INVALID_DOCUMENT", CpfValidator.Validate(request.Cpf).Error);

		var userIdentifier = !string.IsNullOrWhiteSpace(request.Cpf) ? request.Cpf : request.IdContaCorrente.ToString();

		var endpoint = _apiSettings.Endpoints.Login;
		var response = await _httpClient.PostAsJsonAsync(endpoint, request);

		if (!response.IsSuccessStatusCode)
		{
			var errorContent = await response.Content.ReadAsStringAsync();
			var errorResponse = JsonSerializer.Deserialize<HttpErrorResponse>(errorContent);
			_logger.LogError("Erro no login. Status: {StatusCode}, Response: {Error}", response.StatusCode, errorContent);
			throw new CustomExceptions(errorResponse != null ? errorResponse.type : "", "Credenciais inválidas", (int)response.StatusCode);
		}

		var result = await response.Content.ReadFromJsonAsync<AccountLoginResponse>();
		_logger.LogInformation("Login realizado com sucesso. ID: {identifier}", result?.IdContaCorrente);

		// Configura token para outras requisições do sistema
		_tokenService.SetToken(result.Token);

		return (true, "Login realizado com sucesso", result.IdContaCorrente, result.Token);
	}
}
