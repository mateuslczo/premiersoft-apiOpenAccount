using BankMore.OpenAccount.Api.Interfaces;
using BankMore.OpenAccount.Api.Models.Requests;
using BankMore.OpenAccount.Api.Models.Responses;
using BankMore.OpenAccount.Api.Validators.Exceptions;
using BankMore.OpenAccount.Api.Validators.ValueObjects;

public class AccountRegistrationService :IAccountRegistrationService
{
	private readonly HttpClient _httpClient;
	private readonly ITokenService _tokenService;
	private readonly ILogger<AccountRegistrationService> _logger;

	public AccountRegistrationService(
		HttpClient httpClient,
		ITokenService tokenService,
		ILogger<AccountRegistrationService> logger)
	{
		_httpClient = httpClient;
		_tokenService = tokenService;
		_logger = logger;
	}

	public async Task<(bool Success, string Message, Guid? AccountId)> RegisterAsync(OpenAccountRequest request)
	{
		_logger.LogInformation("Iniciando registro de conta para CPF: {Cpf}", request.Cpf);


		var cpfResult = CpfValidator.Validate(request.Cpf);
		if (cpfResult.IsFailure)
			throw new CustomExceptions("INVALID_DOCUMENT", cpfResult.Error);


		var token =  _tokenService.GenerateToken(request.Cpf, request.Senha, "User");
		if (string.IsNullOrEmpty(token))
			throw new CustomExceptions("INVALID_TOKEN", "Token inválido");

		_httpClient.DefaultRequestHeaders.Authorization =
			new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

		var response = await _httpClient.PostAsJsonAsync("api/accounts", request);

		if (!response.IsSuccessStatusCode)
		{
			var errorContent =  response.Content.ReadAsStringAsync();
			_logger.LogError("Erro ao criar conta. Status: {StatusCode}, Response: {Error}", response.StatusCode, errorContent);
			throw new CustomExceptions("USER_UNAUTHORIZED", "Não autorizado", (int)response.StatusCode);

		}

		var created = await response.Content.ReadFromJsonAsync<CreateAccountResponse>();

		_logger.LogInformation("Conta criada com sucesso. ID: {AccountId}", created?.IdContaCorrente);

		return (true, "Conta criada com sucesso.", created?.IdContaCorrente);
	}

}
