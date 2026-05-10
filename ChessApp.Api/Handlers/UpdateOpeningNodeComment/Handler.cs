using ChessApp.API.Data;
using ChessApp.API.DTOs.OpeningNodes;
using Microsoft.EntityFrameworkCore;

namespace ChessApp.API.Handlers.OpeningNodes;

public sealed class UpdateOpeningNodeCommentHandler
{
    private readonly ApplicationDbContext _db;

    public UpdateOpeningNodeCommentHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Execute(
        int userId,
        int openingId,
        int nodeId,
        UpdateOpeningNodeCommentRequest request,
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

        node.Comment = string.IsNullOrWhiteSpace(request.Comment)
            ? null
            : request.Comment.Trim();

        await _db.SaveChangesAsync(cancellationToken);
    }
}