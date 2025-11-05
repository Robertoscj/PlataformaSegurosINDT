namespace ContratacaoService.Domain.Events;

/// <summary>
/// Evento de domínio: Proposta foi aprovada (recebido do PropostaService)
/// </summary>
public class PropostaAprovadaEvent
{
    public Guid PropostaId { get; set; }
    public string NomeCliente { get; set; } = null!;
    public string CpfCliente { get; set; } = null!;
    public string TipoSeguro { get; set; } = null!;
    public decimal ValorCobertura { get; set; }
    public decimal ValorPremio { get; set; }
    public DateTime DataAprovacao { get; set; }
    public string EventType { get; set; } = "PropostaAprovada";
    public DateTime EventTimestamp { get; set; }
}

