using Microsoft.EntityFrameworkCore;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Ports;
using ContratacaoService.Infrastructure.Persistence;

namespace ContratacaoService.Infrastructure.Repositories;

/// <summary>
/// Adapter que implementa o port IContratacaoRepository usando Entity Framework
/// </summary>
public class ContratacaoRepository : IContratacaoRepository
{
    private readonly ContratacaoDbContext _context;

    public ContratacaoRepository(ContratacaoDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Contratacao> CriarAsync(
        Contratacao contratacao,
        CancellationToken cancellationToken = default)
    {
        await _context.Contratacoes.AddAsync(contratacao, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return contratacao;
    }

    public async Task<Contratacao?> ObterPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Contratacoes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Contratacao?> ObterPorPropostaIdAsync(
        Guid propostaId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Contratacoes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.PropostaId == propostaId, cancellationToken);
    }

    public async Task<IEnumerable<Contratacao>> ListarAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Contratacoes
            .AsNoTracking()
            .OrderByDescending(c => c.DataContratacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExisteContratacaoParaPropostaAsync(
        Guid propostaId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Contratacoes
            .AsNoTracking()
            .AnyAsync(c => c.PropostaId == propostaId, cancellationToken);
    }
}

