using ChessApp.API.Enums;
using ChessApp.API.Models;

namespace ChessApp.API.DTOs.OpeningNodes;

public sealed class AddMoveRequest
{
    public int ParentNodeId { get; set; }
    public string MoveUci { get; set; } = null!;
}