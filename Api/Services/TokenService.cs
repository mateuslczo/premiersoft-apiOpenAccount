using BankMore.OpenAccount.Api.Interfaces;
using BankMore.OpenAccount.Api.Validators.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankMore.Infrastructure.Security
{
	public class TokenService :ITokenService
	{
		private readonly IConfiguration _configuration;

		public TokenService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string GenerateToken(string userId, string userPassword, string role)
		{
			try
			{
				var secretKey = _configuration["Jwt:Key"];
				var issuer = _configuration["Jwt:Issuer"];
				var audience = _configuration["Jwt:Audience"];

				if (string.IsNullOrWhiteSpace(secretKey))
				{
					throw new SecurityKeyException(
						"A chave nula ou inexistente");
				}

				if (secretKey.Length < 16)
				{
					throw new SecurityKeyException(
						$"Chave muito curta para o padrão de segurança");
				}

				var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
				var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

				var claims = new[]
				{
				new Claim(JwtRegisteredClaimNames.Sub, userId),
				new Claim(JwtRegisteredClaimNames.UniqueName, userPassword),
				new Claim(ClaimTypes.Role, role),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

				var token = new JwtSecurityToken(
					issuer: issuer,
					audience: audience,
					claims: claims,
					expires: DateTime.UtcNow.AddHours(2),
					signingCredentials: credentials
				);

				return new JwtSecurityTokenHandler().WriteToken(token);
			}
			catch (ArgumentException ex) when (ex.Message.Contains("IDX10703"))
			{

				throw new SecurityKeyException(
					"Falha ao criar chave de segurança JWT: comprimento da chave é zero. ",
					ex);
			}
			catch (Exception ex)
			{

				throw new JwtConfigurationException(
					"Erro de configuração da chave: " + ex.Message,
					ex);
			}
		}


		public bool ValidateToken(string token)
		{
			try
			{
				var secretKey = _configuration["Jwt:Key"];
				var issuer = _configuration["Jwt:Issuer"];
				var audience = _configuration["Jwt:Audience"];

				if (string.IsNullOrWhiteSpace(secretKey))
					throw new SecurityKeyException("A chave JWT não foi configurada.");

				var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

				var tokenHandler = new JwtSecurityTokenHandler();

				tokenHandler.ValidateToken(token, new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = issuer,
					ValidAudience = audience,
					IssuerSigningKey = key,
					ClockSkew = TimeSpan.Zero // evita tolerância de tempo extra
				}, out SecurityToken validatedToken);

				return true;
			}
			catch (SecurityTokenExpiredException)
			{
				// Token expirou
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

	}

}
