namespace ChessApp.API.DTOs.Repertoire;

public sealed class RepertoireItemDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public OpeningColor Color { get; init; }
}