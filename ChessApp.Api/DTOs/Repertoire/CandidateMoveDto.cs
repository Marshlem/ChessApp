using ChessApp.API.Enums;
using ChessApp.API.Models;

namespace ChessApp.API.DTOs.OpeningNodes;

public sealed class CandidateMoveDto
{
    public int NodeId { get; set; }
    public int ParentNodeId { get; set; }

    public string MoveSan { get; set; } = null!;
    public string Fen { get; set; } = null!;

    public LineType LineType { get; set; }
    public string? Comment { get; set; }

    public int TrainedCount { get; set; }
    public int FailedCount { get; set; }
    public DateTime? LastTrainedAtUtc { get; set; }
    public DateTime? NextDueAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
