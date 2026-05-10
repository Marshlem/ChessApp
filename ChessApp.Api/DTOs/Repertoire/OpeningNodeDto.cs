using ChessApp.API.Enums;

namespace ChessApp.API.DTOs.Openings;

public sealed class OpeningNodeDto
{
    public int Id { get; set; }
    public int? ParentNodeId { get; set; }
    public string Fen { get; set; } = null!;
    public string? MoveSan { get; set; }
    public string? MoveUci { get; set; } = null!;
    public string? Comment { get; set; }
    public MoveEvaluation? Evaluation { get; set; }
}
