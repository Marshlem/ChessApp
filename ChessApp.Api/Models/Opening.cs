namespace ChessApp.API.Models;

public sealed class Opening
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public string Name { get; set; } = null!;
    public OpeningColor Color { get; set; }

    public int? RootNodeId { get; set; }
    public OpeningNode? RootNode { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }
}
