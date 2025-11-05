using Microsoft.EntityFrameworkCore;

namespace PropostaService.Infrastructure.Database.Providers;

/// <summary>
/// Provedor para PostgreSQL
/// </summary>
public class PostgreSqlProvider : IDatabaseProvider
{
    public string ProviderName => "PostgreSQL";

    public void Configure(DbContextOptionsBuilder options, string connectionString)
    {
        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null);

            npgsqlOptions.CommandTimeout(30);
        });
    }

    public bool IsAvailable()
    {
        try
        {
            // Verifica se o pacote Npgsql está disponível
            var assembly = typeof(Npgsql.NpgsqlConnection).Assembly;
            return assembly != null;
        }
        catch
        {
            return false;
        }
    }

    public string GetDecimalColumnType() => "decimal(18,2)";

    public string GetTextColumnType() => "text";
}

