using ChessApp.API.Data;
using ChessApp.API.DTOs.OpeningNodes;
using ChessApp.API.Enums;
using ChessApp.API.Infrastructure;
using ChessApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ChessApp.API.Handlers.OpeningNodes;

public sealed class AddMoveHandler
{
    private readonly ApplicationDbContext _db;

    public AddMoveHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<AddMoveResponse> Execute(int userId, int openingId, AddMoveRequest request)
    {
        var parent = await _db.OpeningNodes
            .Include(x => x.Opening)
            .FirstOrDefaultAsync(x =>
                x.Id == request.ParentNodeId &&
                x.OpeningId == openingId &&
                x.Opening.UserId == userId);

        if (parent == null)
            throw new KeyNotFoundException("Parent node not found");

        var (ok, newFen, moveSan) = ChessRules.TryApplyUci(parent.Fen, request.MoveUci);
        if (!ok || newFen == null)
            return AddMoveResponse.IllegalMove();

        // ✅ 1️⃣ ar toks FEN jau egzistuoja?
        var existingNode = await _db.OpeningNodes
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.OpeningId == openingId &&
                x.Fen == newFen);

        if (existingNode != null)
        {
            // 👉 NIEKO nekuriam
            return new AddMoveResponse
            {
                Success = true,
                NodeId = existingNode.Id,
                Fen = existingNode.Fen,
                Reused = true
            };
        }

        // ✅ 2️⃣ kuriam naują node
        var newNode = new OpeningNode
        {
            OpeningId = openingId,
            ParentNodeId = parent.Id,
            Fen = newFen,
            MoveSan = moveSan,
            LineType = LineType.Main,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.OpeningNodes.Add(newNode);
        await _db.SaveChangesAsync();

        return new AddMoveResponse
        {
            Success = true,
            NodeId = newNode.Id,
            Fen = newNode.Fen,
            MoveSan = newNode.MoveSan,
            Reused = false
        };
    }
}
