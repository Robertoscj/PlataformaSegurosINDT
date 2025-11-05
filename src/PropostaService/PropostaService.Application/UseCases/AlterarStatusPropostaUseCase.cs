using PropostaService.Application.DTOs;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Ports;

namespace PropostaService.Application.UseCases;

/// <summary>
/// Caso de uso para alterar o status de uma proposta
/// </summary>
public class AlterarStatusPropostaUseCase
{
    private readonly IPropostaRepository _repository;

    public AlterarStatusPropostaUseCase(IPropostaRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<PropostaResponse> ExecutarAsync(
        Guid id,
        StatusProposta novoStatus,
        CancellationToken cancellationToken = default)
    {
        var proposta = await _repository.ObterPorIdAsync(id, cancellationToken);

        if (proposta == null)
            throw new InvalidOperationException($"Proposta com ID {id} não encontrada");

        proposta.AlterarStatus(novoStatus);

        await _repository.AtualizarAsync(proposta, cancellationToken);

        return MapearParaResponse(proposta);
    }

    private static PropostaResponse MapearParaResponse(Proposta proposta)
    {
        return new PropostaResponse(
            proposta.Id,
            proposta.NomeCliente,
            proposta.CpfCliente.Numero,
            proposta.TipoSeguro,
            proposta.ValorCobertura.Valor,
            proposta.ValorPremio.Valor,
            proposta.Status,
            proposta.DataCriacao,
            proposta.DataAtualizacao
        );
    }
}

