namespace ChessApp.API.Models;

public sealed class RepertoireItem
{
public int Id { get; set; }
public int UserId { get; set; }
public OpeningColor Color { get; set; }
public string Name { get; set; } = null!;

public int? OpeningId { get; set; }
public Opening? Opening { get; set; }
}
