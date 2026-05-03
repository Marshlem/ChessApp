using ChessApp.API.Enums;

namespace ChessApp.API.DTOs.OpeningNodes;

public sealed class UpdateNodeTypeResponse
{
    public int NodeId { get; set; }
    public LineType LineType { get; set; }
}