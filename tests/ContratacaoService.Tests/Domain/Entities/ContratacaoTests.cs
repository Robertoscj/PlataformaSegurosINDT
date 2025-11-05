using FluentAssertions;
using ContratacaoService.Domain.Entities;
using Xunit;

namespace ContratacaoService.Tests.Domain.Entities;

public class ContratacaoTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarContratacao()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var dataInicio = DateTime.UtcNow.AddDays(1);
        var dataFim = DateTime.UtcNow.AddYears(1);
        var valorPremio = 500m;

        // Act
        var contratacao = Contratacao.Criar(propostaId, dataInicio, dataFim, valorPremio);

        // Assert
        contratacao.Should().NotBeNull();
        contratacao.Id.Should().NotBeEmpty();
        contratacao.PropostaId.Should().Be(propostaId);
        contratacao.NumeroApolice.Should().StartWith("APO-");
        contratacao.DataContratacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        contratacao.DataVigenciaInicio.Should().Be(dataInicio);
        contratacao.DataVigenciaFim.Should().Be(dataFim);
        contratacao.ValorPremio.Should().Be(valorPremio);
    }

    [Fact]
    public void Criar_ComPropostaIdVazio_DeveLancarExcecao()
    {
        // Arrange
        var propostaId = Guid.Empty;
        var dataInicio = DateTime.UtcNow.AddDays(1);
        var dataFim = DateTime.UtcNow.AddYears(1);
        var valorPremio = 500m;

        // Act
        Action act = () => Contratacao.Criar(propostaId, dataInicio, dataFim, valorPremio);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*ID da proposta inválido*");
    }

    [Fact]
    public void Criar_ComDataFimAnteriorAInicio_DeveLancarExcecao()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var dataInicio = DateTime.UtcNow.AddYears(1);
        var dataFim = DateTime.UtcNow.AddDays(1);
        var valorPremio = 500m;

        // Act
        Action act = () => Contratacao.Criar(propostaId, dataInicio, dataFim, valorPremio);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Data de fim da vigência deve ser posterior*");
    }

    [Fact]
    public void Criar_ComDataInicioNoPassado_DeveLancarExcecao()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var dataInicio = DateTime.UtcNow.AddDays(-10);
        var dataFim = DateTime.UtcNow.AddYears(1);
        var valorPremio = 500m;

        // Act
        Action act = () => Contratacao.Criar(propostaId, dataInicio, dataFim, valorPremio);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Data de início da vigência não pode ser anterior*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Criar_ComValorPremioInvalido_DeveLancarExcecao(decimal valorInvalido)
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var dataInicio = DateTime.UtcNow.AddDays(1);
        var dataFim = DateTime.UtcNow.AddYears(1);

        // Act
        Action act = () => Contratacao.Criar(propostaId, dataInicio, dataFim, valorInvalido);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Valor do prêmio deve ser maior que zero*");
    }

    [Fact]
    public void Criar_DeveGerarNumeroApoliceUnico()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var dataInicio = DateTime.UtcNow.AddDays(1);
        var dataFim = DateTime.UtcNow.AddYears(1);
        var valorPremio = 500m;

        // Act
        var contratacao1 = Contratacao.Criar(propostaId, dataInicio, dataFim, valorPremio);
        var contratacao2 = Contratacao.Criar(propostaId, dataInicio, dataFim, valorPremio);

        // Assert
        contratacao1.NumeroApolice.Should().NotBe(contratacao2.NumeroApolice);
    }
}

