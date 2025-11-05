using FluentAssertions;
using Moq;
using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.UseCases;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Models;
using ContratacaoService.Domain.Ports;
using Xunit;

namespace ContratacaoService.Tests.Application.UseCases;

public class ContratarPropostaUseCaseTests
{
    private readonly Mock<IContratacaoRepository> _contratacaoRepositoryMock;
    private readonly Mock<IPropostaServiceClient> _propostaServiceClientMock;
    private readonly ContratarPropostaUseCase _useCase;

    public ContratarPropostaUseCaseTests()
    {
        _contratacaoRepositoryMock = new Mock<IContratacaoRepository>();
        _propostaServiceClientMock = new Mock<IPropostaServiceClient>();
        _useCase = new ContratarPropostaUseCase(
            _contratacaoRepositoryMock.Object,
            _propostaServiceClientMock.Object);
    }

    [Fact]
    public async Task ExecutarAsync_ComPropostaAprovada_DeveContratarComSucesso()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var dataInicio = DateTime.UtcNow.AddDays(1);
        var dataFim = DateTime.UtcNow.AddYears(1);

        var request = new ContratarPropostaRequest(propostaId, dataInicio, dataFim);

        var propostaDto = new PropostaDto(
            propostaId,
            "João da Silva",
            "12345678909",
            "Vida",
            100000m,
            500m,
            2, // Aprovada
            DateTime.UtcNow);

        _contratacaoRepositoryMock
            .Setup(r => r.ExisteContratacaoParaPropostaAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _propostaServiceClientMock
            .Setup(c => c.ObterPropostaAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(propostaDto);

        _contratacaoRepositoryMock
            .Setup(r => r.CriarAsync(It.IsAny<Contratacao>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Contratacao c, CancellationToken ct) => c);

        // Act
        var result = await _useCase.ExecutarAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.PropostaId.Should().Be(propostaId);
        result.ValorPremio.Should().Be(500m);
        result.NumeroApolice.Should().StartWith("APO-");

        _contratacaoRepositoryMock.Verify(
            r => r.CriarAsync(It.IsAny<Contratacao>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComPropostaNaoAprovada_DeveLancarExcecao()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var dataInicio = DateTime.UtcNow.AddDays(1);
        var dataFim = DateTime.UtcNow.AddYears(1);

        var request = new ContratarPropostaRequest(propostaId, dataInicio, dataFim);

        var propostaDto = new PropostaDto(
            propostaId,
            "João da Silva",
            "12345678909",
            "Vida",
            100000m,
            500m,
            1, // Em Análise
            DateTime.UtcNow);

        _contratacaoRepositoryMock
            .Setup(r => r.ExisteContratacaoParaPropostaAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _propostaServiceClientMock
            .Setup(c => c.ObterPropostaAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(propostaDto);

        // Act
        Func<Task> act = async () => await _useCase.ExecutarAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Apenas propostas aprovadas podem ser contratadas*");
    }

    [Fact]
    public async Task ExecutarAsync_ComPropostaInexistente_DeveLancarExcecao()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var dataInicio = DateTime.UtcNow.AddDays(1);
        var dataFim = DateTime.UtcNow.AddYears(1);

        var request = new ContratarPropostaRequest(propostaId, dataInicio, dataFim);

        _contratacaoRepositoryMock
            .Setup(r => r.ExisteContratacaoParaPropostaAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _propostaServiceClientMock
            .Setup(c => c.ObterPropostaAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropostaDto?)null);

        // Act
        Func<Task> act = async () => await _useCase.ExecutarAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*Proposta {propostaId} não encontrada no sistema*");
    }

    [Fact]
    public async Task ExecutarAsync_ComContratacaoExistente_DeveLancarExcecao()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var dataInicio = DateTime.UtcNow.AddDays(1);
        var dataFim = DateTime.UtcNow.AddYears(1);

        var request = new ContratarPropostaRequest(propostaId, dataInicio, dataFim);

        _contratacaoRepositoryMock
            .Setup(r => r.ExisteContratacaoParaPropostaAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _useCase.ExecutarAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*Já existe uma contratação para a proposta {propostaId}*");
    }

    [Fact]
    public async Task ExecutarAsync_ComRequestNull_DeveLancarExcecao()
    {
        // Act
        Func<Task> act = async () => await _useCase.ExecutarAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}

