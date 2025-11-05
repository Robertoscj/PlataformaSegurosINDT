using Microsoft.AspNetCore.Mvc;
using PropostaService.Application.DTOs;
using PropostaService.Application.UseCases;
using PropostaService.Domain.Enums;

namespace PropostaService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PropostasController : ControllerBase
{
    private readonly CriarPropostaUseCase _criarPropostaUseCase;
    private readonly ListarPropostasUseCase _listarPropostasUseCase;
    private readonly ObterPropostaUseCase _obterPropostaUseCase;
    private readonly AlterarStatusPropostaUseCase _alterarStatusPropostaUseCase;
    private readonly ILogger<PropostasController> _logger;

    public PropostasController(
        CriarPropostaUseCase criarPropostaUseCase,
        ListarPropostasUseCase listarPropostasUseCase,
        ObterPropostaUseCase obterPropostaUseCase,
        AlterarStatusPropostaUseCase alterarStatusPropostaUseCase,
        ILogger<PropostasController> logger)
    {
        _criarPropostaUseCase = criarPropostaUseCase;
        _listarPropostasUseCase = listarPropostasUseCase;
        _obterPropostaUseCase = obterPropostaUseCase;
        _alterarStatusPropostaUseCase = alterarStatusPropostaUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Cria uma nova proposta de seguro
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PropostaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CriarProposta(
        [FromBody] CriarPropostaRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var proposta = await _criarPropostaUseCase.ExecutarAsync(request, cancellationToken);

            _logger.LogInformation("Proposta criada com sucesso: {PropostaId}", proposta.Id);

            return CreatedAtAction(
                nameof(ObterProposta),
                new { id = proposta.Id },
                proposta);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao criar proposta");
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar proposta");
            return StatusCode(500, new { erro = "Erro interno ao processar a solicitação" });
        }
    }

    /// <summary>
    /// Lista todas as propostas, opcionalmente filtradas por status
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PropostaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListarPropostas(
        [FromQuery] StatusProposta? status,
        CancellationToken cancellationToken)
    {
        try
        {
            var propostas = await _listarPropostasUseCase.ExecutarAsync(status, cancellationToken);
            return Ok(propostas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar propostas");
            return StatusCode(500, new { erro = "Erro interno ao processar a solicitação" });
        }
    }

    /// <summary>
    /// Obtém uma proposta específica por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PropostaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterProposta(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var proposta = await _obterPropostaUseCase.ExecutarAsync(id, cancellationToken);

            if (proposta == null)
            {
                _logger.LogWarning("Proposta não encontrada: {PropostaId}", id);
                return NotFound(new { erro = $"Proposta com ID {id} não encontrada" });
            }

            return Ok(proposta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter proposta {PropostaId}", id);
            return StatusCode(500, new { erro = "Erro interno ao processar a solicitação" });
        }
    }

    /// <summary>
    /// Altera o status de uma proposta
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(PropostaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AlterarStatus(
        Guid id,
        [FromBody] AlterarStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var proposta = await _alterarStatusPropostaUseCase.ExecutarAsync(
                id,
                request.NovoStatus,
                cancellationToken);

            _logger.LogInformation(
                "Status da proposta {PropostaId} alterado para {Status}",
                id,
                request.NovoStatus);

            return Ok(proposta);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro ao alterar status da proposta {PropostaId}", id);
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status da proposta {PropostaId}", id);
            return StatusCode(500, new { erro = "Erro interno ao processar a solicitação" });
        }
    }
}

