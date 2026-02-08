using ChessApp.API.Data;
using ChessApp.API.DTOs.OpeningNodes;
using Microsoft.EntityFrameworkCore;

public sealed class GetCandidateMovesQuery
{
    private readonly ApplicationDbContext _db;

    public GetCandidateMovesQuery(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<CandidateMoveDto>> Execute(int userId, string fen, int? currentOpeningId)
    {
        // 1. randam VISUS node'us su šita FEN
        var parentNodes = await _db.OpeningNodes
            .Where(n => n.Fen == fen)
            .Select(n => new { n.Id, n.OpeningId })
            .ToListAsync();

        if (parentNodes.Count == 0)
            return [];

        var parentNodeIds = parentNodes.Select(x => x.Id).ToList();

        // 2. imam jų vaikus (candidate moves)
        var moves = await _db.OpeningNodes
            .Where(n => parentNodeIds.Contains(n.ParentNodeId!.Value))
            .Join(
                _db.Openings,
                n => n.OpeningId,
                o => o.Id,
                (n, o) => new { Node = n, Opening = o }
            )
            .Where(x => x.Opening.UserId == userId)
            .Select(x => new CandidateMoveDto
            {
                OpeningId = x.Opening.Id,
                OpeningName = x.Opening.Name,
                MoveSan = x.Node.MoveSan!,
                MoveUci = x.Node.MoveUci!,
                IsFromCurrentOpening =
                    currentOpeningId != null &&
                    x.Opening.Id == currentOpeningId
            })
            .Distinct()
            .OrderBy(x => x.MoveSan)
            .ToListAsync();

        return moves;
    }
}
