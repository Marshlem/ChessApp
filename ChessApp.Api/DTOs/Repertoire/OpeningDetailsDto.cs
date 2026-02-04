namespace ChessApp.API.DTOs.Openings;

public sealed class OpeningDetailsDto
{
    public int OpeningId { get; set; }
    public int? RootNodeId { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public List<RepertoireBreadcrumbDto> Breadcrumbs { get; set; } = new();

    public OpeningNodeDto RootNode { get; set; } = null!;
}