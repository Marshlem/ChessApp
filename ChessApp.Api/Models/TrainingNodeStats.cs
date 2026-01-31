namespace ChessApp.API.Models;

public sealed class TrainingNodeStats
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid OpeningNodeId { get; set; }
    public OpeningNode OpeningNode { get; set; } = null!;

    public int TrainedCount { get; set; }
    public int FailedCount { get; set; }

    public DateTime? LastTrainedAtUtc { get; set; }
    public DateTime? NextDueAtUtc { get; set; }
}
