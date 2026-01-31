namespace ChessApp.API.Models;

public sealed class RepertoireItem
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public Guid? ParentId { get; set; }
    public RepertoireItem? Parent { get; set; }
    public List<RepertoireItem> Children { get; set; } = new();

    public RepertoireItemType Type { get; set; } 
    public OpeningColor Color { get; set; }     

    public string Name { get; set; } = null!;
    public int SortOrder { get; set; }

    public Guid? OpeningId { get; set; }
    public Opening? Opening { get; set; }
}
