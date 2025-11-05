using FluentAssertions;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using Xunit;

namespace PropostaService.Tests.Domain.Entities;

public class PropostaTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarProposta()
    {
        // Arrange
        var nomeCliente = "João da Silva";
        var cpfCliente = "12345678909";
        var tipoSeguro = "Vida";
        var valorCobertura = 100000m;
        var valorPremio = 500m;

        // Act
        var proposta = Proposta.Criar(
            nomeCliente,
            cpfCliente,
            tipoSeguro,
            valorCobertura,
            valorPremio);

        // Assert
        proposta.Should().NotBeNull();
        proposta.Id.Should().NotBeEmpty();
        proposta.NomeCliente.Should().Be(nomeCliente);
        proposta.TipoSeguro.Should().Be(tipoSeguro);
        proposta.Status.Should().Be(StatusProposta.EmAnalise);
        proposta.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Jo")]
    public void Criar_ComNomeInvalido_DeveLancarExcecao(string nomeInvalido)
    {
        // Act
        Action act = () => Proposta.Criar(
            nomeInvalido,
            "12345678909",
            "Vida",
            100000m,
            500m);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("Saude")]
    [InlineData("Invalido")]
    public void Criar_ComTipoSeguroInvalido_DeveLancarExcecao(string tipoInvalido)
    {
        // Act
        Action act = () => Proposta.Criar(
            "João da Silva",
            "12345678909",
            tipoInvalido,
            100000m,
            500m);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AlterarStatus_DeEmAnaliseParaAprovada_DeveAlterarStatus()
    {
        // Arrange
        var proposta = Proposta.Criar(
            "João da Silva",
            "12345678909",
            "Vida",
            100000m,
            500m);

        // Act
        proposta.AlterarStatus(StatusProposta.Aprovada);

        // Assert
        proposta.Status.Should().Be(StatusProposta.Aprovada);
        proposta.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public void AlterarStatus_ParaMesmoStatus_DeveLancarExcecao()
    {
        // Arrange
        var proposta = Proposta.Criar(
            "João da Silva",
            "12345678909",
            "Vida",
            100000m,
            500m);

        // Act
        Action act = () => proposta.AlterarStatus(StatusProposta.EmAnalise);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*já está com o status*");
    }

    [Fact]
    public void AlterarStatus_QuandoJaAprovada_DeveLancarExcecao()
    {
        // Arrange
        var proposta = Proposta.Criar(
            "João da Silva",
            "12345678909",
            "Vida",
            100000m,
            500m);
        proposta.AlterarStatus(StatusProposta.Aprovada);

        // Act
        Action act = () => proposta.AlterarStatus(StatusProposta.Rejeitada);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*proposta já finalizada*");
    }

    [Fact]
    public void PodeSerContratada_QuandoAprovada_DeveRetornarTrue()
    {
        // Arrange
        var proposta = Proposta.Criar(
            "João da Silva",
            "12345678909",
            "Vida",
            100000m,
            500m);
        proposta.AlterarStatus(StatusProposta.Aprovada);

        // Act
        var podeSerContratada = proposta.PodeSerContratada();

        // Assert
        podeSerContratada.Should().BeTrue();
    }

    [Theory]
    [InlineData(StatusProposta.EmAnalise)]
    [InlineData(StatusProposta.Rejeitada)]
    public void PodeSerContratada_QuandoNaoAprovada_DeveRetornarFalse(StatusProposta status)
    {
        // Arrange
        var proposta = Proposta.Criar(
            "João da Silva",
            "12345678909",
            "Vida",
            100000m,
            500m);

        if (status == StatusProposta.Rejeitada)
            proposta.AlterarStatus(StatusProposta.Rejeitada);

        // Act
        var podeSerContratada = proposta.PodeSerContratada();

        // Assert
        podeSerContratada.Should().BeFalse();
    }
}

