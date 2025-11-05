using Microsoft.AspNetCore.Mvc;
using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.UseCases;

namespace ContratacaoService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ContratacoesController : ControllerBase
{
    private readonly ContratarPropostaUseCase _contratarPropostaUseCase;
    private readonly ListarContratacoesUseCase _listarContratacoesUseCase;
    private readonly ObterContratacaoUseCase _obterContratacaoUseCase;
    private readonly ILogger<ContratacoesController> _logger;

    public ContratacoesController(
        ContratarPropostaUseCase contratarPropostaUseCase,
        ListarContratacoesUseCase listarContratacoesUseCase,
        ObterContratacaoUseCase obterContratacaoUseCase,
        ILogger<ContratacoesController> logger)
    {
        _contratarPropostaUseCase = contratarPropostaUseCase;
        _listarContratacoesUseCase = listarContratacoesUseCase;
        _obterContratacaoUseCase = obterContratacaoUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Contrata uma proposta aprovada
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ContratacaoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ContratarProposta(
        [FromBody] ContratarPropostaRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var contratacao = await _contratarPropostaUseCase.ExecutarAsync(request, cancellationToken);

            _logger.LogInformation(
                "Contratação criada com sucesso: {ContratacaoId} para proposta {PropostaId}",
                contratacao.Id,
                contratacao.PropostaId);

            return CreatedAtAction(
                nameof(ObterContratacao),
                new { id = contratacao.Id },
                contratacao);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao contratar proposta");
            return BadRequest(new { erro = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao contratar proposta");
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao contratar proposta");
            return StatusCode(500, new { erro = "Erro interno ao processar a solicitação" });
        }
    }

    /// <summary>
    /// Lista todas as contratações
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ContratacaoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListarContratacoes(CancellationToken cancellationToken)
    {
        try
        {
            var contratacoes = await _listarContratacoesUseCase.ExecutarAsync(cancellationToken);
            return Ok(contratacoes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar contratações");
            return StatusCode(500, new { erro = "Erro interno ao processar a solicitação" });
        }
    }

    /// <summary>
    /// Obtém uma contratação específica por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContratacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterContratacao(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var contratacao = await _obterContratacaoUseCase.ExecutarAsync(id, cancellationToken);

            if (contratacao == null)
            {
                _logger.LogWarning("Contratação não encontrada: {ContratacaoId}", id);
                return NotFound(new { erro = $"Contratação com ID {id} não encontrada" });
            }

            return Ok(contratacao);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter contratação {ContratacaoId}", id);
            return StatusCode(500, new { erro = "Erro interno ao processar a solicitação" });
        }
    }
}

