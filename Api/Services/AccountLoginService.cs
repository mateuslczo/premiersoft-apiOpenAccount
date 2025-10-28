using BankMore.OpenAccount.Api.Interfaces;
using BankMore.OpenAccount.Api.Models.Requests;
using BankMore.OpenAccount.Api.Models.Responses;
using BankMore.OpenAccount.Api.Validators.Exceptions;
using BankMore.OpenAccount.Api.Validators.ValueObjects;

public class AccountLoginService :IAccountLoginService
{
	private readonly HttpClient _httpClient;
	private readonly ITokenService _tokenService;
	private readonly ILogger<AccountLoginService> _logger;

	public AccountLoginService(
		HttpClient httpClient,
		ITokenService tokenService,
		ILogger<AccountLoginService> logger)
	{
		_httpClient = httpClient;
		_tokenService = tokenService;
		_logger = logger;
	}

	public async Task<(bool Success, string Message, Guid? AccountId, string Token)> LoginAccountAsync(AccountLoginRequest request)
	{
		_logger.LogInformation("Iniciando login para: {Cpf} ou ID: {Id}", request.Cpf, request.IdContaCorrente);

		if (string.IsNullOrWhiteSpace(request.Cpf) && request.IdContaCorrente == Guid.Empty)
			throw new CustomExceptions("INVALID_CREDENTIALS", "CPF ou ID da conta é obrigatório");

		if (!string.IsNullOrWhiteSpace(request.Cpf) && CpfValidator.Validate(request.Cpf).IsFailure)
			throw new CustomExceptions("INVALID_DOCUMENT", CpfValidator.Validate(request.Cpf).Error);

		var userIdentifier = !string.IsNullOrWhiteSpace(request.Cpf) ? request.Cpf : request.IdContaCorrente.ToString();

		var token = _tokenService.GenerateToken(userIdentifier, request.Senha, "User")
			?? throw new CustomExceptions("INVALID_TOKEN", "Token inválido");


		_httpClient.DefaultRequestHeaders.Authorization =
			new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

		var response = await _httpClient.PostAsJsonAsync("api/accounts/login", request);

		if (!response.IsSuccessStatusCode)
		{
			var errorContent =  response.Content.ReadAsStringAsync();
			_logger.LogError("Erro no login. Status: {StatusCode}, Response: {Error}", response.StatusCode, errorContent);
			throw new CustomExceptions("LOGIN_FAILED", "Credenciais inválidas", (int)response.StatusCode);
		}

		var result = await response.Content.ReadFromJsonAsync<AccountLoginResponse>();
		_logger.LogInformation("Login realizado com sucesso. ID: {identifier}", result?.IdContaCorrente);

		return (true, "Login realizado com sucesso", result?.IdContaCorrente, result?.Token);
	}


}
