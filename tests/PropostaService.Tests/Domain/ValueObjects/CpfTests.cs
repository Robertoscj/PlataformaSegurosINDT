using FluentAssertions;
using PropostaService.Domain.ValueObjects;
using Xunit;

namespace PropostaService.Tests.Domain.ValueObjects;

public class CpfTests
{
    [Theory]
    [InlineData("12345678909")]
    [InlineData("123.456.789-09")]
    [InlineData("529.982.247-25")]
    public void Criar_ComCpfValido_DeveRetornarCpf(string cpfValido)
    {
        // Act
        var cpf = Cpf.Criar(cpfValido);

        // Assert
        cpf.Should().NotBeNull();
        cpf.Numero.Should().HaveLength(11);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComCpfVazio_DeveLancarExcecao(string cpfInvalido)
    {
        // Act
        Action act = () => Cpf.Criar(cpfInvalido);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*CPF não pode ser vazio*");
    }

    [Theory]
    [InlineData("12345678900")]
    [InlineData("11111111111")]
    [InlineData("00000000000")]
    [InlineData("123")]
    public void Criar_ComCpfInvalido_DeveLancarExcecao(string cpfInvalido)
    {
        // Act
        Action act = () => Cpf.Criar(cpfInvalido);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*CPF inválido*");
    }

    [Fact]
    public void FormatarCpf_DeveRetornarCpfFormatado()
    {
        // Arrange
        var cpf = Cpf.Criar("12345678909");

        // Act
        var cpfFormatado = cpf.FormatarCpf();

        // Assert
        cpfFormatado.Should().Be("123.456.789-09");
    }

    [Fact]
    public void Equals_ComMesmoCpf_DeveRetornarTrue()
    {
        // Arrange
        var cpf1 = Cpf.Criar("12345678909");
        var cpf2 = Cpf.Criar("123.456.789-09");

        // Act & Assert
        cpf1.Equals(cpf2).Should().BeTrue();
    }

    [Fact]
    public void Equals_ComCpfsDiferentes_DeveRetornarFalse()
    {
        // Arrange
        var cpf1 = Cpf.Criar("12345678909");
        var cpf2 = Cpf.Criar("52998224725");

        // Act & Assert
        cpf1.Equals(cpf2).Should().BeFalse();
    }
}

