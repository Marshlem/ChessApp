using ChessApp.API.Enums;

namespace ChessApp.API.Models;

public sealed class OpeningNode
{
    public int Id { get; set; }

    public int OpeningId { get; set; }
    public Opening Opening { get; set; } = null!;

    public int? ParentNodeId { get; set; }
    public OpeningNode? ParentNode { get; set; }
    public List<OpeningNode> Children { get; set; } = new();

    public string Fen { get; set; } = null!;
    public string? MoveSan { get; set; }

    public LineType LineType { get; set; }
    public string? Comment { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public string? MoveUci { get; set; } = null!;
}
