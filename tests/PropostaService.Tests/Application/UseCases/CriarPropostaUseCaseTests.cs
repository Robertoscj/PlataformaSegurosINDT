using FluentAssertions;
using Moq;
using PropostaService.Application.DTOs;
using PropostaService.Application.UseCases;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Ports;
using Xunit;

namespace PropostaService.Tests.Application.UseCases;

public class CriarPropostaUseCaseTests
{
    private readonly Mock<IPropostaRepository> _repositoryMock;
    private readonly CriarPropostaUseCase _useCase;

    public CriarPropostaUseCaseTests()
    {
        _repositoryMock = new Mock<IPropostaRepository>();
        _useCase = new CriarPropostaUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task ExecutarAsync_ComDadosValidos_DeveCriarProposta()
    {
        // Arrange
        var request = new CriarPropostaRequest(
            "João da Silva",
            "12345678909",
            "Vida",
            100000m,
            500m);

        _repositoryMock
            .Setup(r => r.CriarAsync(It.IsAny<Proposta>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Proposta p, CancellationToken ct) => p);

        // Act
        var result = await _useCase.ExecutarAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.NomeCliente.Should().Be("João da Silva");
        result.TipoSeguro.Should().Be("Vida");
        result.Status.Should().Be(StatusProposta.EmAnalise);
        result.ValorCobertura.Should().Be(100000m);
        result.ValorPremio.Should().Be(500m);

        _repositoryMock.Verify(
            r => r.CriarAsync(It.IsAny<Proposta>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComRequestNull_DeveLancarExcecao()
    {
        // Act
        Func<Task> act = async () => await _useCase.ExecutarAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task ExecutarAsync_ComDadosInvalidos_DeveLancarExcecao()
    {
        // Arrange
        var request = new CriarPropostaRequest(
            "", // Nome inválido
            "12345678909",
            "Vida",
            100000m,
            500m);

        // Act
        Func<Task> act = async () => await _useCase.ExecutarAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}

