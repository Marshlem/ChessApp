using ChessApp.API.Enums;

namespace ChessApp.API.DTOs.OpeningNodes;

public sealed class UpdateNodeTypeRequest
{
    public LineType LineType { get; set; }
}