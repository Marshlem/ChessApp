namespace ChessApp.API.Models;

public sealed class Opening
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string Name { get; set; } = null!;
    public OpeningColor Color { get; set; }

    public Guid RootNodeId { get; set; }
    public OpeningNode RootNode { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }
}
