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
                x.CreatedAtUtc,
                x.Color
            })
            .FirstOrDefaultAsync();

        if (opening == null)
            throw new KeyNotFoundException("Opening not found");

        var rootNode = await _db.OpeningNodes
            .AsNoTracking()
            .Where(n => n.Id == opening.RootNodeId && n.OpeningId == openingId)
            .Select(n => new OpeningNodeDto
            {
                Id = n.Id,
                ParentNodeId = n.ParentNodeId,
                Fen = n.Fen,
                MoveSan = n.MoveSan,
                LineType = (int)n.LineType,
                Comment = n.Comment,
                Training = _db.TrainingNodeStats
                    .AsNoTracking()
                    .Where(s => s.UserId == userId && s.OpeningNodeId == n.Id)
                    .Select(s => new TrainingNodeStatsDto
                    {
                        TrainedCount = s.TrainedCount,
                        FailedCount = s.FailedCount,
                        LastTrainedAtUtc = s.LastTrainedAtUtc,
                        NextDueAtUtc = s.NextDueAtUtc
                    })
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        if (rootNode == null)
            throw new KeyNotFoundException("Root node not found");

        var breadcrumbs = new List<RepertoireBreadcrumbDto>
        {
            new()
            {
                Id = opening.Id,
                Name = opening.Color == OpeningColor.White ? "White" : "Black",
                SortOrder = 0
            }
        };

        return new OpeningDetailsDto
        {
            OpeningId = opening.Id,
            RootNodeId = opening.RootNodeId,
            CreatedAtUtc = opening.CreatedAtUtc,
            Breadcrumbs = breadcrumbs,
            RootNode = rootNode
        };
    }
}
