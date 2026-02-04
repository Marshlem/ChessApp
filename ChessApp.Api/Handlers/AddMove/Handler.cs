using ChessApp.API.Data;
using ChessApp.API.DTOs.OpeningNodes;
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

    public async Task<int> Execute(int userId, int openingId, AddMoveRequest request)
    {
        var parent = await _db.OpeningNodes
            .AsNoTracking()
            .Where(x => x.Id == request.ParentNodeId)
            .Select(x => new
            {
                x.Id,
                x.OpeningId,
                OpeningUserId = x.Opening.UserId
            })
            .FirstOrDefaultAsync();

        if (parent == null)
            throw new InvalidOperationException("Parent node not found.");

        if (parent.OpeningId != openingId)
            throw new InvalidOperationException("Parent node does not belong to this opening.");

        if (parent.OpeningUserId != userId)
            throw new UnauthorizedAccessException("Access denied.");

        var exists = await _db.OpeningNodes.AnyAsync(x =>
            x.OpeningId == openingId &&
            x.ParentNodeId == request.ParentNodeId &&
            x.MoveSan == request.MoveSan);

        if (exists)
            throw new InvalidOperationException("This move already exists in this position.");

        var node = new OpeningNode
        {
            Id = _db.OpeningNodes.Max(x => x.Id) + 1,
            OpeningId = openingId,
            ParentNodeId = request.ParentNodeId,
            Fen = request.FenAfter.Trim(),
            MoveSan = request.MoveSan.Trim(),
            LineType = request.LineType,
            Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.OpeningNodes.Add(node);

        var statsExists = await _db.TrainingNodeStats.AnyAsync(x =>
            x.UserId == userId && x.OpeningNodeId == node.Id);

        if (!statsExists)
        {
            _db.TrainingNodeStats.Add(new TrainingNodeStats
            {
                Id = _db.TrainingNodeStats.Max(x => x.Id) + 1,
                UserId = userId,
                OpeningNodeId = node.Id,
                TrainedCount = 0,
                FailedCount = 0,
                LastTrainedAtUtc = null,
                NextDueAtUtc = null
            });
        }

        await _db.SaveChangesAsync();

        return node.Id;
    }
}
