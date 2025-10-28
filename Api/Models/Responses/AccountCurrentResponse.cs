using System.Security.Cryptography;
using System.Text;

namespace BankMore.OpenAccount.Api.Models.Responses
{
	public class AccountCurrentResponse
	{
		public Guid IdContaCorrente { get; private set; }
		public int Numero { get; private set; }
		public string Nome { get; private set; }
		public bool Ativo { get; private set; }
		public string Senha { get; private set; }
		public string Salt { get; private set; }
		public decimal Saldo { get; set; } = 0M;

		/// <summary>
		/// Valida a senha informada comparando o hash gerado com o hash armazenado.
		/// </summary>
		public bool ValidarSenha(string senhaInformada)
		{
			if (string.IsNullOrEmpty(senhaInformada) || string.IsNullOrEmpty(Salt) || string.IsNullOrEmpty(Senha))
				return false;

			using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(Salt));
			var senhaHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(senhaInformada)));

			return senhaHash == Senha;
		}
	}
}