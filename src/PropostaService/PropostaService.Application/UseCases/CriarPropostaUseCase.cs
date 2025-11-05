using PropostaService.Application.DTOs;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Ports;

namespace PropostaService.Application.UseCases;

/// <summary>
/// Caso de uso para criar uma nova proposta de seguro
/// </summary>
public class CriarPropostaUseCase
{
    private readonly IPropostaRepository _repository;

    public CriarPropostaUseCase(IPropostaRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<PropostaResponse> ExecutarAsync(
        CriarPropostaRequest request, 
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var proposta = Proposta.Criar(
            request.NomeCliente,
            request.CpfCliente,
            request.TipoSeguro,
            request.ValorCobertura,
            request.ValorPremio
        );

        var propostaCriada = await _repository.CriarAsync(proposta, cancellationToken);

        return MapearParaResponse(propostaCriada);
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

