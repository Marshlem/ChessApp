namespace ChessApp.API.DTOs.Openings;

public sealed class TrainingNodeStatsDto
{
    public int TrainedCount { get; set; }
    public int FailedCount { get; set; }
    public DateTime? LastTrainedAtUtc { get; set; }
    public DateTime? NextDueAtUtc { get; set; }
}