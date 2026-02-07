namespace ChessApp.API.DTOs.Repertoire;

public sealed class RepertoireItemDto
{
    public int Id { get; init; }
    public int? OpeningId { get; set; }
    public string Name { get; init; } = null!;
    public OpeningColor Color { get; init; }
}