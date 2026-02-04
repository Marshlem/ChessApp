namespace ChessApp.API.Models;

public sealed class TrainingNodeStats
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int OpeningNodeId { get; set; }
    public OpeningNode OpeningNode { get; set; } = null!;

    public int TrainedCount { get; set; }
    public int FailedCount { get; set; }

    public DateTime? LastTrainedAtUtc { get; set; }
    public DateTime? NextDueAtUtc { get; set; }
}
