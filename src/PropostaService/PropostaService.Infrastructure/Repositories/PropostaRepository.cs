using Microsoft.EntityFrameworkCore;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Ports;
using PropostaService.Infrastructure.Persistence;

namespace PropostaService.Infrastructure.Repositories;

/// <summary>
/// Adapter que implementa o port IPropostaRepository usando Entity Framework
/// </summary>
public class PropostaRepository : IPropostaRepository
{
    private readonly PropostaDbContext _context;

    public PropostaRepository(PropostaDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Proposta> CriarAsync(Proposta proposta, CancellationToken cancellationToken = default)
    {
        await _context.Propostas.AddAsync(proposta, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return proposta;
    }

    public async Task<Proposta?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Propostas
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Proposta>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Propostas
            .AsNoTracking()
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Proposta>> ListarPorStatusAsync(
        StatusProposta status,
        CancellationToken cancellationToken = default)
    {
        return await _context.Propostas
            .AsNoTracking()
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Proposta proposta, CancellationToken cancellationToken = default)
    {
        _context.Propostas.Update(proposta);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExisteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Propostas
            .AsNoTracking()
            .AnyAsync(p => p.Id == id, cancellationToken);
    }
}

