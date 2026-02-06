using ChessApp.API.Enums;
using ChessApp.API.Models;

namespace ChessApp.API.DTOs.OpeningNodes;

public sealed class AddMoveResponse
{
    public bool Success { get; set; }
    public int? NodeId { get; set; }
    public string? Fen { get; set; }
    public bool Reused { get; set; }
    public string? Error { get; set; }
    public string? MoveSan { get; set; }

    public static AddMoveResponse IllegalMove() => new()
    {
        Success = false,
        Error = "Illegal move"
    };
}