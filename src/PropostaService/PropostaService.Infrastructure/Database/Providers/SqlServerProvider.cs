using Microsoft.EntityFrameworkCore;

namespace PropostaService.Infrastructure.Database.Providers;

/// <summary>
/// Provedor para SQL Server
/// </summary>
public class SqlServerProvider : IDatabaseProvider
{
    public string ProviderName => "SqlServer";

    public void Configure(DbContextOptionsBuilder options, string connectionString)
    {
        options.UseSqlServer(connectionString, sqlServerOptions =>
        {
            sqlServerOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);

            sqlServerOptions.CommandTimeout(30);

            // Configurações específicas do SQL Server
            sqlServerOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
    }

    public bool IsAvailable()
    {
        try
        {
            // Verifica se o pacote Microsoft.EntityFrameworkCore.SqlServer está disponível
            var type = Type.GetType("Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions, Microsoft.EntityFrameworkCore.SqlServer");
            return type != null;
        }
        catch
        {
            return false;
        }
    }

    public string GetDecimalColumnType() => "decimal(18,2)";

    public string GetTextColumnType() => "nvarchar(max)";
}

