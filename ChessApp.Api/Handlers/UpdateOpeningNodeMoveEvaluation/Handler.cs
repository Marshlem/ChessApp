using ChessApp.API.Data;
using ChessApp.API.DTOs.OpeningNodes;
using Microsoft.EntityFrameworkCore;

namespace ChessApp.API.Handlers.OpeningNodes;

public sealed class UpdateOpeningNodeMoveEvaluationHandler
{
    private readonly ApplicationDbContext _db;

    public UpdateOpeningNodeMoveEvaluationHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Execute(
        int userId,
        int openingId,
        int nodeId,
        UpdateOpeningNodeMoveEvaluationRequest request,
        CancellationToken cancellationToken)
    {
        var node = await _db.OpeningNodes
            .Include(x => x.Opening)
            .FirstOrDefaultAsync(x =>
                    x.Id == nodeId &&
                    x.OpeningId == openingId &&
                    x.Opening.UserId == userId,
                cancellationToken);

        if (node == null)
            throw new KeyNotFoundException("Opening node not found");

        node.Evaluation = request.Evaluation;

        await _db.SaveChangesAsync(cancellationToken);
    }
}