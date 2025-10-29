namespace BankMore.OpenAccount.Api.Models.Requests
{
	public class AccountTransactionRequest
	{

		public int IdContaCorrente { get; set; }

		public string TipoMovimento { get; set; } = string.Empty;

		public decimal Valor { get; set; }

		public decimal SaldoAnterior { get; set; }

		public decimal SaldoAtual { get; set; }

		public string Senha { get; set; }

		public string Descricao { get; set; } = "Movimentação simples D/C";

		public DateTime DataMovimento { get; set; } = DateTime.Now;
	}
}