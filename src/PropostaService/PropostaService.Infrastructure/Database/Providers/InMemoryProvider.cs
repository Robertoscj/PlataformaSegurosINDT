using Microsoft.EntityFrameworkCore;

namespace PropostaService.Infrastructure.Database.Providers;

/// <summary>
/// Provedor InMemory para testes
/// </summary>
public class InMemoryProvider : IDatabaseProvider
{
    public string ProviderName => "InMemory";

    public void Configure(DbContextOptionsBuilder options, string connectionString)
    {
        // Para InMemory, connectionString é usado como nome do banco
        var databaseName = string.IsNullOrEmpty(connectionString)
            ? "PropostaTestDb"
            : connectionString;

        options.UseInMemoryDatabase(databaseName);
    }

    public bool IsAvailable()
    {

        return true;
    }

    public string GetDecimalColumnType() => "decimal(18,2)";

    public string GetTextColumnType() => "text";
}

