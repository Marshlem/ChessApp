using ChessApp.API.Data;
using ChessApp.API.DTOs.Repertoire;
using ChessApp.API.Enums;
using ChessApp.API.Models;
using ChessApp.API.Constants;

namespace ChessApp.API.Handlers.Repertoire;
public sealed class CreateOpeningHandler
{
    private readonly ApplicationDbContext _db;

    public CreateOpeningHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<int> Execute(int userId, CreateOpeningRequest request)
    {
        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");

        await using var tx = await _db.Database.BeginTransactionAsync();

        var opening = new Opening
        {
            UserId = userId,
            Name = name,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Openings.Add(opening);
        await _db.SaveChangesAsync();

        var rootNode = new OpeningNode
        {
            OpeningId = opening.Id,
            ParentNodeId = null,
            Fen = FenConstants.StartFen,
            LineType = LineType.Main,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.OpeningNodes.Add(rootNode);
        await _db.SaveChangesAsync();

        opening.RootNodeId = rootNode.Id;

        var item = new RepertoireItem
        {
            UserId = userId,
            Color = request.Color,
            Name = name,
            OpeningId = opening.Id
        };

        _db.RepertoireItems.Add(item);

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        return opening.Id;
    }
}
