using System.Text.Json;
using ContratacaoService.Domain.Models;
using ContratacaoService.Domain.Ports;
using Microsoft.Extensions.Logging;

namespace ContratacaoService.Infrastructure.ExternalServices;

/// <summary>
/// Adapter que implementa o port IPropostaServiceClient usando HttpClient
/// Comunicação entre microserviços via HTTP
/// </summary>
public class PropostaServiceClient : IPropostaServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PropostaServiceClient> _logger;

    public PropostaServiceClient(
        HttpClient httpClient, 
        ILogger<PropostaServiceClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PropostaDto?> ObterPropostaAsync(
        Guid propostaId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Buscando proposta {PropostaId} no PropostaService", propostaId);

            var response = await _httpClient.GetAsync(
                $"api/propostas/{propostaId}", 
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Proposta {PropostaId} não encontrada", propostaId);
                    return null;
                }

                _logger.LogError(
                    "Erro ao buscar proposta {PropostaId}. Status: {StatusCode}", 
                    propostaId, 
                    response.StatusCode);
                
                throw new HttpRequestException(
                    $"Erro ao comunicar com PropostaService. Status: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var proposta = JsonSerializer.Deserialize<PropostaDto>(content, options);

            _logger.LogInformation(
                "Proposta {PropostaId} obtida com sucesso. Status: {Status}", 
                propostaId, 
                proposta?.Status);

            return proposta;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de comunicação ao buscar proposta {PropostaId}", propostaId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao buscar proposta {PropostaId}", propostaId);
            throw;
        }
    }
}

