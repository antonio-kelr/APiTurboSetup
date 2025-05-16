using System.ComponentModel.DataAnnotations;

namespace APiTurboSetup.Validations
{
    public class CpfValidation
    {
        public static bool IsValid(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                return false;

            // Remove caracteres não numéricos
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
                return false;

            // Verifica se todos os dígitos são iguais
            if (new string(cpf[0], 11) == cpf)
                return false;

            // Validação do primeiro dígito verificador
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += int.Parse(cpf[i].ToString()) * (10 - i);

            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            if (digito1 != int.Parse(cpf[9].ToString()))
                return false;

            // Validação do segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(cpf[i].ToString()) * (11 - i);

            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            return digito2 == int.Parse(cpf[10].ToString());
        }
    }
} 