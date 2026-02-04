using ChessApp.API.DTOs.Repertoire;

namespace ChessApp.API.DTOs.Openings;

public sealed class OpeningNodeDto
{
    public int Id { get; set; }
    public int? ParentNodeId { get; set; }

    public string Fen { get; set; } = null!;
    public string? MoveSan { get; set; }

    public int LineType { get; set; } 
    public string? Comment { get; set; }

    public TrainingNodeStatsDto? Training { get; set; }
}