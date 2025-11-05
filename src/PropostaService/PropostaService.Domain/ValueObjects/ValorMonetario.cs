namespace PropostaService.Domain.ValueObjects;

/// <summary>
/// Value Object para representar valores monetários
/// </summary>
public sealed class ValorMonetario
{
    public decimal Valor { get; private set; }

    private ValorMonetario(decimal valor)
    {
        Valor = valor;
    }

    public static ValorMonetario Criar(decimal valor)
    {
        if (valor < 0)
            throw new ArgumentException("Valor não pode ser negativo", nameof(valor));

        return new ValorMonetario(valor);
    }

    public override string ToString() => Valor.ToString("C2");

    public override bool Equals(object? obj)
    {
        if (obj is not ValorMonetario outroValor)
            return false;

        return Valor == outroValor.Valor;
    }

    public override int GetHashCode() => Valor.GetHashCode();

    public static implicit operator decimal(ValorMonetario valorMonetario) => valorMonetario.Valor;
}

