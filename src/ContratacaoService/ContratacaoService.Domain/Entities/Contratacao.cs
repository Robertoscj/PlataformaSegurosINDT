namespace ContratacaoService.Domain.Entities;

/// <summary>
/// Entidade que representa uma contratação de seguro
/// </summary>
public class Contratacao
{
    public Guid Id { get; private set; }
    public Guid PropostaId { get; private set; }
    public string NumeroApolice { get; private set; } = null!;
    public DateTime DataContratacao { get; private set; }
    public DateTime DataVigenciaInicio { get; private set; }
    public DateTime DataVigenciaFim { get; private set; }
    public decimal ValorPremio { get; private set; }

    // Construtor para EF Core
    private Contratacao() { }

    private Contratacao(
        Guid propostaId,
        string numeroApolice,
        DateTime dataVigenciaInicio,
        DateTime dataVigenciaFim,
        decimal valorPremio)
    {
        Id = Guid.NewGuid();
        PropostaId = propostaId;
        NumeroApolice = numeroApolice;
        DataContratacao = DateTime.UtcNow;
        DataVigenciaInicio = dataVigenciaInicio;
        DataVigenciaFim = dataVigenciaFim;
        ValorPremio = valorPremio;
    }

    public static Contratacao Criar(
        Guid propostaId,
        DateTime dataVigenciaInicio,
        DateTime dataVigenciaFim,
        decimal valorPremio)
    {
        ValidarPropostaId(propostaId);
        ValidarVigencia(dataVigenciaInicio, dataVigenciaFim);
        ValidarValorPremio(valorPremio);

        var numeroApolice = GerarNumeroApolice();

        return new Contratacao(
            propostaId,
            numeroApolice,
            dataVigenciaInicio,
            dataVigenciaFim,
            valorPremio);
    }

    private static void ValidarPropostaId(Guid propostaId)
    {
        if (propostaId == Guid.Empty)
            throw new ArgumentException("ID da proposta inválido", nameof(propostaId));
    }

    private static void ValidarVigencia(DateTime inicio, DateTime fim)
    {
        if (inicio >= fim)
            throw new ArgumentException("Data de fim da vigência deve ser posterior à data de início");

        if (inicio < DateTime.UtcNow.Date)
            throw new ArgumentException("Data de início da vigência não pode ser anterior à data atual");
    }

    private static void ValidarValorPremio(decimal valor)
    {
        if (valor <= 0)
            throw new ArgumentException("Valor do prêmio deve ser maior que zero", nameof(valor));
    }

    private static string GerarNumeroApolice()
    {
        // Gera um número de apólice no formato: APO-YYYYMMDD-XXXXXX
        var data = DateTime.UtcNow.ToString("yyyyMMdd");
        var sequencial = Guid.NewGuid().ToString("N")[..6].ToUpper();
        return $"APO-{data}-{sequencial}";
    }
}

