using ChessApp.API.Models;

namespace ChessApp.API.DTOs.Repertoire;

public sealed class CreateOpeningRequest
{
    public OpeningColor Color { get; set; }
    public string Name { get; set; } = null!;
}
