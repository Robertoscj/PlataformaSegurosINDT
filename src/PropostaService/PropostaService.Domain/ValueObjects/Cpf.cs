namespace PropostaService.Domain.ValueObjects;

/// <summary>
/// Value Object para representar um CPF
/// </summary>
public sealed class Cpf
{
    public string Numero { get; private set; }

    private Cpf(string numero)
    {
        Numero = numero;
    }

    public static Cpf Criar(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("CPF não pode ser vazio", nameof(numero));

        var cpfLimpo = LimparCpf(numero);

        if (!ValidarCpf(cpfLimpo))
            throw new ArgumentException("CPF inválido", nameof(numero));

        return new Cpf(cpfLimpo);
    }

    private static string LimparCpf(string cpf)
    {
        return new string(cpf.Where(char.IsDigit).ToArray());
    }

    private static bool ValidarCpf(string cpf)
    {
        if (cpf.Length != 11)
            return false;

        // Verifica se todos os dígitos são iguais
        if (cpf.Distinct().Count() == 1)
            return false;

        // Valida primeiro dígito verificador
        var soma = 0;
        for (int i = 0; i < 9; i++)
            soma += int.Parse(cpf[i].ToString()) * (10 - i);

        var resto = soma % 11;
        var digitoVerificador1 = resto < 2 ? 0 : 11 - resto;

        if (int.Parse(cpf[9].ToString()) != digitoVerificador1)
            return false;

        // Valida segundo dígito verificador
        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += int.Parse(cpf[i].ToString()) * (11 - i);

        resto = soma % 11;
        var digitoVerificador2 = resto < 2 ? 0 : 11 - resto;

        return int.Parse(cpf[10].ToString()) == digitoVerificador2;
    }

    public override string ToString() => Numero;

    public string FormatarCpf()
    {
        return $"{Numero.Substring(0, 3)}.{Numero.Substring(3, 3)}.{Numero.Substring(6, 3)}-{Numero.Substring(9, 2)}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Cpf outroCpf)
            return false;

        return Numero == outroCpf.Numero;
    }

    public override int GetHashCode() => Numero.GetHashCode();
}

