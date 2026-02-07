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

        if (!ChessRules.TryApplyUci(
                parent.Fen,
                request.MoveUci,
                out var newFen,
                out var moveSan))
        {
            return AddMoveResponse.IllegalMove();
        }

        // ✅ deduplikacija pagal FEN
        var existingNode = await _db.OpeningNodes
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.OpeningId == openingId &&
                x.Fen == newFen);

        if (existingNode != null)
        {
            return new AddMoveResponse
            {
                Success = true,
                NodeId = existingNode.Id,
                Fen = existingNode.Fen,
                Reused = true
            };
        }

        var newNode = new OpeningNode
        {
            OpeningId = openingId,
            ParentNodeId = parent.Id,
            Fen = newFen,
            MoveUci = request.MoveUci,
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
