using PropostaService.Application.DTOs;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Ports;

namespace PropostaService.Application.UseCases;

/// <summary>
/// Caso de uso para obter uma proposta específica por ID
/// </summary>
public class ObterPropostaUseCase
{
    private readonly IPropostaRepository _repository;

    public ObterPropostaUseCase(IPropostaRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<PropostaResponse?> ExecutarAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var proposta = await _repository.ObterPorIdAsync(id, cancellationToken);

        return proposta == null ? null : MapearParaResponse(proposta);
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

