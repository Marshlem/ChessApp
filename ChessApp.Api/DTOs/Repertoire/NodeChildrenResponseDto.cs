namespace ChessApp.API.DTOs.OpeningNodes;

public sealed class NodeChildrenResponseDto
{
    public int NodeId { get; set; }
    public string Fen { get; set; } = null!;

    public List<CandidateMoveDto> Children { get; set; } = new();
}