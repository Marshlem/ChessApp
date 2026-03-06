using MediatR;
using Microsoft.EntityFrameworkCore;
using ChessApp.API.Data;
using ChessApp.API.Enums;

namespace ChessApp.Application.Training.GetNextTrainingPosition;

public sealed class GetNextTrainingPositionHandler
    : IRequestHandler<GetNextTrainingPositionQuery, GetNextTrainingPositionResponse?>
{
    private readonly ApplicationDbContext _dbContext;

    public GetNextTrainingPositionHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetNextTrainingPositionResponse?> Handle(
        GetNextTrainingPositionQuery request,
        CancellationToken cancellationToken)
    {
        var nowUtc = DateTime.UtcNow;

        var repertoireOpenings = _dbContext.RepertoireItems
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId && x.OpeningId.HasValue)
            .Select(x => new
            {
                OpeningId = x.OpeningId!.Value,
                RepertoireColor = x.Color
            });

        var candidate = await
            (from node in _dbContext.OpeningNodes.AsNoTracking()
             join opening in _dbContext.Openings.AsNoTracking()
                 on node.OpeningId equals opening.Id
             join repertoire in repertoireOpenings
                 on node.OpeningId equals repertoire.OpeningId
             join stats in _dbContext.TrainingNodeStats.AsNoTracking()
                     .Where(x => x.UserId == request.UserId)
                 on node.Id equals stats.OpeningNodeId into statsGroup
             from stats in statsGroup.DefaultIfEmpty()
             where _dbContext.OpeningNodes.Count(x => x.ParentNodeId == node.Id && x.LineType == LineType.Main) == 1
             where stats == null || stats.NextDueAtUtc == null || stats.NextDueAtUtc <= nowUtc
             orderby
                 stats == null ? 0 : 1,
                 stats == null ? 0 : stats.Bucket,
                 stats == null ? DateTime.MinValue : (stats.NextDueAtUtc ?? DateTime.MinValue),
                 node.Id
             select new
             {
                 node.Id,
                 node.OpeningId,
                 opening.Name,
                 node.Fen,
                 repertoire.RepertoireColor
             })
            .FirstOrDefaultAsync(cancellationToken);

        if (candidate is null)
        {
            return null;
        }

        var trainedMove = await _dbContext.OpeningNodes
            .AsNoTracking()
            .Where(x => x.ParentNodeId == candidate.Id && x.LineType == LineType.Main)
            .Select(x => new TrainingMoveOptionDto
            {
                OpeningNodeId = x.Id,
                MoveSan = x.MoveSan ?? string.Empty,
                MoveUci = x.MoveUci
            })
            .SingleAsync(cancellationToken);

        return new GetNextTrainingPositionResponse
        {
            OpeningNodeId = candidate.Id,
            OpeningId = candidate.OpeningId,
            OpeningName = candidate.Name,
            Fen = candidate.Fen,
            SideToMove = GetSideToMoveFromFen(candidate.Fen),
            RepertoireColor = (int)candidate.RepertoireColor,
            MoveOptions = new List<TrainingMoveOptionDto> { trainedMove }
        };
    }

    private static string GetSideToMoveFromFen(string fen)
    {
        if (string.IsNullOrWhiteSpace(fen))
        {
            return string.Empty;
        }

        var parts = fen.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
        {
            return string.Empty;
        }

        return parts[1] switch
        {
            "w" => "w",
            "b" => "b",
            _ => string.Empty
        };
    }
}