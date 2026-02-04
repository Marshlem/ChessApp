using ChessApp.API.DTOs.OpeningNodes;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ChessApp.API.Data;

public sealed class OpeningNodeReadRepository
{
    private readonly ApplicationDbContext _db;

    public OpeningNodeReadRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<CandidateMoveDto>> GetCandidateMoves(
        int userId,
        int openingId,
        int parentNodeId)
    {
        return await _db.Set<CandidateMoveDto>()
            .FromSqlRaw(
                "SELECT * FROM get_candidate_moves(@u, @o, @p)",
                new NpgsqlParameter("u", userId),
                new NpgsqlParameter("o", openingId),
                new NpgsqlParameter("p", parentNodeId)
            )
            .AsNoTracking()
            .ToListAsync();
    }
}
