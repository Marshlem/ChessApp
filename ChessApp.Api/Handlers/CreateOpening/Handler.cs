using ChessApp.API.Constants;
using ChessApp.API.Data;
using ChessApp.API.DTOs.Repertoire;
using ChessApp.API.Enums;
using ChessApp.API.Models;

namespace ChessApp.API.Handlers.Repertoire;

public sealed class CreateOpeningHandler
{
    private readonly ApplicationDbContext _db;
    private readonly PgnImportService _pgnImportService;

    public CreateOpeningHandler(
        ApplicationDbContext db,
        PgnImportService pgnImportService)
    {
        _db = db;
        _pgnImportService = pgnImportService;
    }

    public async Task<int> Execute(
        int userId,
        CreateOpeningRequest request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");

        var opening = new Opening
        {
            UserId = userId,
            Name = name,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Openings.Add(opening);
        await _db.SaveChangesAsync(cancellationToken); // reikia opening.Id

        var rootNode = new OpeningNode
        {
            OpeningId = opening.Id,
            Fen = FenConstants.StartFen,
            LineType = LineType.Main,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.OpeningNodes.Add(rootNode);
        await _db.SaveChangesAsync(cancellationToken); // reikia rootNode.Id

        opening.RootNodeId = rootNode.Id;

        var repertoireItem = new RepertoireItem
        {
            UserId = userId,
            Name = name,
            Color = request.Color,
            OpeningId = opening.Id
        };

        _db.RepertoireItems.Add(repertoireItem);
        await _db.SaveChangesAsync(cancellationToken); // kaip buvo sename variante

        if (!string.IsNullOrWhiteSpace(request.PgnText))
        {
            await _pgnImportService.Import(
                openingId: opening.Id,
                rootNode: rootNode,
                pgnText: request.PgnText,
                cancellationToken: cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);
        }

        return opening.Id;
    }
}