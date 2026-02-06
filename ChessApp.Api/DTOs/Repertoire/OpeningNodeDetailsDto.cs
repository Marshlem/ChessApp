using ChessApp.API.DTOs.Openings;
using ChessApp.API.Enums;

namespace ChessApp.API.DTOs.OpeningNodes;

public sealed class OpeningNodeDetailsDto
{
    public int NodeId { get; set; }
    public string Fen { get; set; } = null!;
    public string? MoveSan { get; set; }

    public LineType LineType { get; set; }
    public string? Comment { get; set; }

    public TrainingNodeStatsDto? Training { get; set; }
}
