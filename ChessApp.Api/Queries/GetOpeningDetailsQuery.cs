using ChessApp.API.Data;
using ChessApp.API.DTOs.Openings;
using Microsoft.EntityFrameworkCore;

namespace ChessApp.API.Queries.Openings;

public sealed class GetOpeningDetailsQuery
{
    private readonly ApplicationDbContext _db;

    public GetOpeningDetailsQuery(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<OpeningDetailsDto> Execute(int userId, int openingId)
    {
        var opening = await _db.Openings
            .AsNoTracking()
            .Where(x => x.Id == openingId && x.UserId == userId)
            .Select(x => new
            {
                x.Id,
                x.RootNodeId,
                x.CreatedAtUtc
            })
            .FirstOrDefaultAsync();

        var color = await _db.RepertoireItems
            .Where(r => r.OpeningId == openingId && r.UserId == userId)
            .Select(r => r.Color)
            .SingleAsync();

        if (opening == null)
            throw new KeyNotFoundException("Opening not found");

        if (opening.RootNodeId == null)
            throw new InvalidOperationException("Opening has no root node");

        var breadcrumbs = new List<RepertoireBreadcrumbDto>
        {
            new()
            {
                Id = opening.Id,
                Color = color
            }
        };

        var nodes = await _db.OpeningNodes
            .AsNoTracking()
            .Where(x => x.OpeningId == openingId)
            .Select(x => new OpeningNodeDto
            {
                Id = x.Id,
                ParentNodeId = x.ParentNodeId,
                Fen = x.Fen,
                MoveSan = x.MoveSan,
                MoveUci = x.MoveUci
            })
            .ToListAsync();

        return new OpeningDetailsDto
        {
            OpeningId = opening.Id,
            RootNodeId = opening.RootNodeId.Value,
            CreatedAtUtc = opening.CreatedAtUtc,
            Color = color,
            Breadcrumbs = breadcrumbs,
            Nodes = nodes
        };
    }
}
