using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;

namespace BankMore.OpenAccount.Api.Models.Responses
{
	public class AccountCurrentResponse
	{
		public int IdContaCorrente { get; set; }
		public int Numero { get; set; }
		public string Nome { get; set; }
		public bool Ativo { get; set; }
		public string Senha { get; set; }
		public string Salt { get; set; }
		public decimal Saldo { get; set; } = 0M;


		public bool ValidarSenha(string senhaInformada)
		{
			if (string.IsNullOrEmpty(senhaInformada) || string.IsNullOrEmpty(Salt) || string.IsNullOrEmpty(Senha))
				return false;

			// Converte o salt armazenado (Base64) de volta para bytes
			var saltBytes = Convert.FromBase64String(Salt);

			// Gera o hash da senha informada usando os mesmos parâmetros
			using var pbkdf2 = new Rfc2898DeriveBytes(
				password: senhaInformada,
				salt: saltBytes,
				iterations: 100_000,
				hashAlgorithm: HashAlgorithmName.SHA256);

			var hashBytes = pbkdf2.GetBytes(32);
			var senhaHash = Convert.ToBase64String(hashBytes);

			// Compara o hash gerado com o hash armazenado
			return senhaHash == Senha;
		}

	
	}
}