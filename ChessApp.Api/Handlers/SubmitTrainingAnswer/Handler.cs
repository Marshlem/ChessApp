using ChessApp.API.Data;
using ChessApp.API.Enums;
using ChessApp.API.Models;
using ChessApp.Application.Training.GetNextTrainingPosition;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChessApp.Application.Training.SubmitTrainingAnswer;

public sealed class SubmitTrainingAnswerHandler
    : IRequestHandler<SubmitTrainingAnswerCommand, SubmitTrainingAnswerResponse>
{
    private readonly ApplicationDbContext _dbContext;

    public SubmitTrainingAnswerHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SubmitTrainingAnswerResponse> Handle(
        SubmitTrainingAnswerCommand request,
        CancellationToken cancellationToken)
    {
        var nowUtc = DateTime.UtcNow;

        var currentNode = await _dbContext.OpeningNodes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.OpeningNodeId, cancellationToken);

        if (currentNode is null)
        {
            throw new InvalidOperationException("Training position was not found.");
        }

        var childNodes = await _dbContext.OpeningNodes
            .Where(x => x.ParentNodeId == request.OpeningNodeId)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        if (childNodes.Count == 0)
        {
            throw new InvalidOperationException("Training position does not have any candidate moves.");
        }

        var selectedNode = childNodes.FirstOrDefault(x => x.Id == request.SelectedOpeningNodeId);
        if (selectedNode is null)
        {
            throw new InvalidOperationException("Selected move does not belong to the training position.");
        }

        var mainLineNodes = childNodes
            .Where(x => x.LineType == LineType.Main)
            .ToList();

        if (mainLineNodes.Count == 0)
        {
            throw new InvalidOperationException("Training position does not have a main line move.");
        }

        if (mainLineNodes.Count > 1)
        {
            throw new InvalidOperationException("Training position has more than one main line move.");
        }

        var correctNode = mainLineNodes[0];
        var isCorrect = selectedNode.Id == correctNode.Id;

        var stats = await _dbContext.TrainingNodeStats
            .FirstOrDefaultAsync(
                x => x.UserId == request.UserId && x.OpeningNodeId == request.OpeningNodeId,
                cancellationToken);

        if (stats is null)
        {
            stats = new TrainingNodeStats
            {
                UserId = request.UserId,
                OpeningNodeId = request.OpeningNodeId,
                Bucket = 0,
                TrainedCount = 0,
                FailedCount = 0
            };

            _dbContext.TrainingNodeStats.Add(stats);
        }

        stats.LastTrainedAtUtc = nowUtc;

        if (isCorrect)
        {
            stats.TrainedCount++;
            stats.Bucket++;
        }
        else
        {
            stats.FailedCount++;
            stats.Bucket = Math.Max(0, stats.Bucket - 1);
        }

        stats.NextDueAtUtc = nowUtc.Add(GetBucketDelay(stats.Bucket));

        await _dbContext.SaveChangesAsync(cancellationToken);

        var nextMoveOptions = await _dbContext.OpeningNodes
            .AsNoTracking()
            .Where(x => x.ParentNodeId == selectedNode.Id && x.LineType == LineType.Main)
            .Select(x => new TrainingMoveOptionDto
            {
                OpeningNodeId = x.Id,
                MoveSan = x.MoveSan ?? string.Empty,
                MoveUci = x.MoveUci
            })
            .ToListAsync(cancellationToken);

        return new SubmitTrainingAnswerResponse
        {
            IsCorrect = isCorrect,
            CorrectOpeningNodeId = correctNode.Id,
            CorrectMoveSan = correctNode.MoveSan ?? string.Empty,
            CorrectMoveUci = correctNode.MoveUci,
            Bucket = stats.Bucket,
            TrainedCount = stats.TrainedCount,
            FailedCount = stats.FailedCount,
            NextDueAtUtc = stats.NextDueAtUtc,
            CurrentOpeningNodeId = selectedNode.Id,
            CurrentFen = selectedNode.Fen,
            CurrentSideToMove = GetSideToMoveFromFen(selectedNode.Fen),
            MoveOptions = nextMoveOptions
        };
    }

    private static TimeSpan GetBucketDelay(int bucket)
    {
        return bucket switch
        {
            <= 0 => TimeSpan.FromMinutes(10),
            1 => TimeSpan.FromHours(4),
            2 => TimeSpan.FromDays(1),
            3 => TimeSpan.FromDays(3),
            4 => TimeSpan.FromDays(7),
            5 => TimeSpan.FromDays(14),
            _ => TimeSpan.FromDays(30)
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