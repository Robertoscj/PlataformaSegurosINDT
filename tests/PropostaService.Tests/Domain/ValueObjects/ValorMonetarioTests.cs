using FluentAssertions;
using PropostaService.Domain.ValueObjects;
using Xunit;

namespace PropostaService.Tests.Domain.ValueObjects;

public class ValorMonetarioTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(100.50)]
    [InlineData(1000000)]
    public void Criar_ComValorValido_DeveRetornarValorMonetario(decimal valor)
    {
        // Act
        var valorMonetario = ValorMonetario.Criar(valor);

        // Assert
        valorMonetario.Should().NotBeNull();
        valorMonetario.Valor.Should().Be(valor);
    }

    [Fact]
    public void Criar_ComValorNegativo_DeveLancarExcecao()
    {
        // Arrange
        decimal valorNegativo = -10;

        // Act
        Action act = () => ValorMonetario.Criar(valorNegativo);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Valor não pode ser negativo*");
    }

    [Fact]
    public void Equals_ComMesmoValor_DeveRetornarTrue()
    {
        // Arrange
        var valor1 = ValorMonetario.Criar(100.50m);
        var valor2 = ValorMonetario.Criar(100.50m);

        // Act & Assert
        valor1.Equals(valor2).Should().BeTrue();
    }

    [Fact]
    public void Equals_ComValoresDiferentes_DeveRetornarFalse()
    {
        // Arrange
        var valor1 = ValorMonetario.Criar(100.50m);
        var valor2 = ValorMonetario.Criar(200.75m);

        // Act & Assert
        valor1.Equals(valor2).Should().BeFalse();
    }

    [Fact]
    public void ImplicitConversion_DeveConverterParaDecimal()
    {
        // Arrange
        var valorMonetario = ValorMonetario.Criar(150.75m);

        // Act
        decimal valorDecimal = valorMonetario;

        // Assert
        valorDecimal.Should().Be(150.75m);
    }
}

