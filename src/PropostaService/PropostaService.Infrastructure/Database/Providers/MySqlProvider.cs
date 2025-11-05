using Microsoft.EntityFrameworkCore;

namespace PropostaService.Infrastructure.Database.Providers;

/// <summary>
/// Provedor para MySQL/MariaDB
/// </summary>
public class MySqlProvider : IDatabaseProvider
{
    public string ProviderName => "MySql";

    public void Configure(DbContextOptionsBuilder options, string connectionString)
    {
        // Para MySQL, use Pomelo.EntityFrameworkCore.MySql
        options.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString),
            mySqlOptions =>
            {
                mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);

                mySqlOptions.CommandTimeout(30);
            });
    }

    public bool IsAvailable()
    {
        try
        {
            // Verifica se o pacote Pomelo.EntityFrameworkCore.MySql está disponível
            var type = Type.GetType("Microsoft.EntityFrameworkCore.MySqlDbContextOptionsBuilderExtensions, Pomelo.EntityFrameworkCore.MySql");
            return type != null;
        }
        catch
        {
            return false;
        }
    }

    public string GetDecimalColumnType() => "decimal(18,2)";

    public string GetTextColumnType() => "longtext";
}

