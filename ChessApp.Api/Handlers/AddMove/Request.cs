using ChessApp.API.Enums;
using ChessApp.API.Models;

namespace ChessApp.API.DTOs.OpeningNodes;

public sealed class AddMoveRequest
{
    public int ParentNodeId { get; set; }
    public string MoveSan { get; set; } = null!;
    public string FenAfter { get; set; } = null!;
    public LineType LineType { get; set; } = LineType.Main;
    public string? Comment { get; set; }
}
