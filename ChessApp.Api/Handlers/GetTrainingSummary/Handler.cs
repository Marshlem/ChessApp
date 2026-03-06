using ChessApp.API.Data;
using ChessApp.API.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChessApp.Application.Training.GetTrainingSummary;

public sealed class GetTrainingSummaryHandler
    : IRequestHandler<GetTrainingSummaryQuery, GetTrainingSummaryResponse>
{
    private readonly ApplicationDbContext _dbContext;

    public GetTrainingSummaryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetTrainingSummaryResponse> Handle(
        GetTrainingSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var nowUtc = DateTime.UtcNow;

        var repertoireOpeningIds = _dbContext.RepertoireItems
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId && x.OpeningId.HasValue)
            .Select(x => x.OpeningId!.Value);

        var trainableNodeIdsQuery =
            _dbContext.OpeningNodes
                .AsNoTracking()
                .Where(node => repertoireOpeningIds.Contains(node.OpeningId))
                .Where(node => _dbContext.OpeningNodes.Any(x => x.ParentNodeId == node.Id))
                .Where(node => _dbContext.OpeningNodes.Count(x => x.ParentNodeId == node.Id && x.LineType == LineType.Main) == 1)
                .Select(node => node.Id);

        var totalPositions = await trainableNodeIdsQuery.CountAsync(cancellationToken);

        var userStatsQuery = _dbContext.TrainingNodeStats
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId);

        var newPositions = await
            (from nodeId in trainableNodeIdsQuery
             join stats in userStatsQuery
                 on nodeId equals stats.OpeningNodeId into statsGroup
             from stats in statsGroup.DefaultIfEmpty()
             where stats == null
             select nodeId)
            .CountAsync(cancellationToken);

        var duePositions = await
            (from nodeId in trainableNodeIdsQuery
             join stats in userStatsQuery
                 on nodeId equals stats.OpeningNodeId into statsGroup
             from stats in statsGroup.DefaultIfEmpty()
             where stats == null || stats.NextDueAtUtc == null || stats.NextDueAtUtc <= nowUtc
             select nodeId)
            .CountAsync(cancellationToken);

        return new GetTrainingSummaryResponse
        {
            TotalPositions = totalPositions,
            NewPositions = newPositions,
            DuePositions = duePositions
        };
    }
}