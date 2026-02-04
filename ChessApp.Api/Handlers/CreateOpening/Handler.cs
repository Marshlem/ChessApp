using ChessApp.API.Data;
using ChessApp.API.DTOs.Repertoire;
using ChessApp.API.Enums;
using ChessApp.API.Models;
using Microsoft.EntityFrameworkCore;

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

        var sortOrder =
            await _db.RepertoireItems
                .Where(x => x.UserId == userId && x.Color == request.Color)
                .Select(x => (int?)x.SortOrder)
                .MaxAsync() ?? 0;

        var opening = new Opening
        {
            UserId = userId,
            Name = name,
            CreatedAtUtc = DateTime.UtcNow
        };

        var rootNode = new OpeningNode
        {
            Opening = opening,
            Fen = "startpos",
            LineType = LineType.Main,
            CreatedAtUtc = DateTime.UtcNow
        };

        var item = new RepertoireItem
        {
            UserId = userId,
            Type = RepertoireItemType.Opening,
            Color = request.Color,
            Name = name,
            SortOrder = sortOrder + 1,
            Opening = opening
        };

        _db.AddRange(opening, rootNode, item);
        await _db.SaveChangesAsync(); 

        opening.RootNodeId = rootNode.Id;
        await _db.SaveChangesAsync(); 

        return opening.Id;
    }
}
