using PropostaService.Application.DTOs;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Ports;

namespace PropostaService.Application.UseCases;


public class ListarPropostasUseCase
{
    private readonly IPropostaRepository _repository;

    public ListarPropostasUseCase(IPropostaRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<PropostaResponse>> ExecutarAsync(
        StatusProposta? status = null,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Proposta> propostas;

        if (status.HasValue)
        {
            propostas = await _repository.ListarPorStatusAsync(status.Value, cancellationToken);
        }
        else
        {
            propostas = await _repository.ListarAsync(cancellationToken);
        }

        return propostas.Select(MapearParaResponse);
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

