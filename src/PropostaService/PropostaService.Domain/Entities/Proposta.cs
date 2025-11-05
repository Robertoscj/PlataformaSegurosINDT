using PropostaService.Domain.Enums;
using PropostaService.Domain.ValueObjects;

namespace PropostaService.Domain.Entities;

/// <summary>
/// Entidade que representa uma proposta de seguro
/// </summary>
public class Proposta
{
    public Guid Id { get; private set; }
    public string NomeCliente { get; private set; } = null!;
    public Cpf CpfCliente { get; private set; } = null!;
    public string TipoSeguro { get; private set; } = null!;
    public ValorMonetario ValorCobertura { get; private set; } = null!;
    public ValorMonetario ValorPremio { get; private set; } = null!;
    public StatusProposta Status { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }

    // Construtor para EF Core
    private Proposta() { }

    private Proposta(
        string nomeCliente,
        Cpf cpfCliente,
        string tipoSeguro,
        ValorMonetario valorCobertura,
        ValorMonetario valorPremio)
    {
        Id = Guid.NewGuid();
        NomeCliente = nomeCliente;
        CpfCliente = cpfCliente;
        TipoSeguro = tipoSeguro;
        ValorCobertura = valorCobertura;
        ValorPremio = valorPremio;
        Status = StatusProposta.EmAnalise;
        DataCriacao = DateTime.UtcNow;
    }

    public static Proposta Criar(
        string nomeCliente,
        string cpfCliente,
        string tipoSeguro,
        decimal valorCobertura,
        decimal valorPremio)
    {
        ValidarNomeCliente(nomeCliente);
        ValidarTipoSeguro(tipoSeguro);

        var cpf = Cpf.Criar(cpfCliente);
        var valorCoberturaObj = ValorMonetario.Criar(valorCobertura);
        var valorPremioObj = ValorMonetario.Criar(valorPremio);

        return new Proposta(nomeCliente, cpf, tipoSeguro, valorCoberturaObj, valorPremioObj);
    }

    public void AlterarStatus(StatusProposta novoStatus)
    {
        if (Status == novoStatus)
            throw new InvalidOperationException($"A proposta já está com o status {novoStatus}");

        // Regras de negócio para transição de status
        if (Status == StatusProposta.Aprovada || Status == StatusProposta.Rejeitada)
            throw new InvalidOperationException(
                "Não é possível alterar o status de uma proposta já finalizada");

        Status = novoStatus;
        DataAtualizacao = DateTime.UtcNow;
    }

    public bool PodeSerContratada()
    {
        return Status == StatusProposta.Aprovada;
    }

    private static void ValidarNomeCliente(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do cliente é obrigatório", nameof(nome));

        if (nome.Length < 3)
            throw new ArgumentException("Nome do cliente deve ter pelo menos 3 caracteres", nameof(nome));
    }

    private static void ValidarTipoSeguro(string tipo)
    {
        if (string.IsNullOrWhiteSpace(tipo))
            throw new ArgumentException("Tipo de seguro é obrigatório", nameof(tipo));

        var tiposValidos = new[] { "Vida", "Auto", "Residencial", "Empresarial", "Viagem" };
        if (!tiposValidos.Contains(tipo, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException(
                $"Tipo de seguro inválido. Tipos válidos: {string.Join(", ", tiposValidos)}", 
                nameof(tipo));
    }
}

