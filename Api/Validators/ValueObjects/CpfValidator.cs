using CSharpFunctionalExtensions;
namespace BankMore.OpenAccount.Api.Validators.ValueObjects
{

	public sealed class CpfValidator
	{
		public string CpfNumber { get; }

		private CpfValidator(string digits)
		{
			CpfNumber = digits;
		}

		public static Result Validate(string digits)
		{
			if (!isValid(digits))
				return Result.Failure("INVALID_DOCUMENT");

			return Result.Success(new CpfValidator(Clear(digits)));
		}

		private static bool isValid(string cpf)
		{
			cpf = Clear(cpf);

			if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
				return false;

			var numeros = cpf.Select(c => int.Parse(c.ToString())).ToArray();
			int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
			int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

			int soma = 0;
			for (int i = 0; i < 9; i++)
				soma += numeros[i] * multiplicador1[i];

			int resto = soma % 11;
			int digito1 = resto < 2 ? 0 : 11 - resto;

			soma = 0;
			for (int i = 0; i < 10; i++)
				soma += numeros[i] * multiplicador2[i];

			resto = soma % 11;
			int digito2 = resto < 2 ? 0 : 11 - resto;

			return numeros[9] == digito1 && numeros[10] == digito2;
		}

		private static string Clear(string cpf) =>
			new string(cpf.Where(char.IsDigit).ToArray());
	}
}