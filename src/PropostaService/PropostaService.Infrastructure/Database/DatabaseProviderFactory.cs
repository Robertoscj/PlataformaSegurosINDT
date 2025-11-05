using PropostaService.Infrastructure.Database.Providers;

namespace PropostaService.Infrastructure.Database;

/// <summary>
/// Factory para criação de provedores de banco de dados
/// Padrão Factory para facilitar a troca de providers
/// </summary>
public static class DatabaseProviderFactory
{
    private static readonly Dictionary<string, Func<IDatabaseProvider>> _providers = new()
    {
        { "postgresql", () => new PostgreSqlProvider() },
        { "postgres", () => new PostgreSqlProvider() },
        { "npgsql", () => new PostgreSqlProvider() },

        { "sqlserver", () => new SqlServerProvider() },
        { "mssql", () => new SqlServerProvider() },
        { "sql", () => new SqlServerProvider() },

        { "mysql", () => new MySqlProvider() },
        { "mariadb", () => new MySqlProvider() },

        { "inmemory", () => new InMemoryProvider() },
        { "memory", () => new InMemoryProvider() }
    };

    /// <summary>
    /// Cria um provedor baseado no nome
    /// </summary>
    public static IDatabaseProvider Create(string providerName)
    {
        if (string.IsNullOrWhiteSpace(providerName))
            throw new ArgumentException("Nome do provedor não pode ser vazio", nameof(providerName));

        var normalizedName = providerName.ToLowerInvariant().Trim();

        if (_providers.TryGetValue(normalizedName, out var factory))
        {
            var provider = factory();

            if (!provider.IsAvailable())
            {
                throw new InvalidOperationException(
                    $"Provedor '{providerName}' não está disponível. " +
                    $"Instale o pacote NuGet correspondente.");
            }

            return provider;
        }

        throw new NotSupportedException(
            $"Provedor de banco de dados '{providerName}' não é suportado. " +
            $"Provedores disponíveis: {string.Join(", ", _providers.Keys)}");
    }

    /// <summary>
    /// Lista todos os provedores disponíveis
    /// </summary>
    public static IEnumerable<string> GetAvailableProviders()
    {
        return _providers.Keys.Distinct().OrderBy(k => k);
    }

    /// <summary>
    /// Verifica se um provedor está disponível
    /// </summary>
    public static bool IsProviderAvailable(string providerName)
    {
        try
        {
            var provider = Create(providerName);
            return provider.IsAvailable();
        }
        catch
        {
            return false;
        }
    }
}

